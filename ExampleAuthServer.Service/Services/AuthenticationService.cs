using ExampleAuthServer.Core.Configuration;
using ExampleAuthServer.Core.Dtos;
using ExampleAuthServer.Core.Models;
using ExampleAuthServer.Core.Repositories;
using ExampleAuthServer.Core.Services; // Kütüphane olan IAuthenticationService değil bizim oluşturduğumuz !
using ExampleAuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;
        public AuthenticationService(IOptions<List<Client>> optionsClient, ITokenService tokenService, UserManager<UserApp> userManager,
            IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionsClient.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            //Login bilgileri yoksa null dön.
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));
            //Identity email üzerinden bilgilere ulaş.
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            //Eğer bu bilgilere ait kullanıcı yoksa hata dön.
            if (user == null) return Response<TokenDto>.Fail("Email or password is wrong", 400, true);
            //Kullanıcı varsa email üzerinden bulduğumuz user ile loginDto'dan gelen password kontrolünü sağla.Uymuyorsa hata dön.
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or password is wrong", 400, true);
            }
            //Tüm kontroller tamamsa bulunan user için token oluştur.Bu tokenı en son return ediyoruz.
            var token = _tokenService.CreateToken(user);
            //Refresh Token yoksa oluştur varsa güncelle.
            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken
                { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }
            //Kaydet
            await _unitOfWork.CommitAsync();
            // Token'ı 200 kodu ile birlikte dön.
            return Response<TokenDto>.Success(token, 200);
        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto) //Client dediğimiz Spa,mobil vs. user değil.
        {
            var client = _clients.SingleOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);
            if (client == null)
            {
                return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", 404, true);
            }
            var token = _tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            //Refresh token kontrolü yap.
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            //Refresh token null ise hata dön
            if (existRefreshToken == null) { return Response<TokenDto>.Fail("Refresh token not found", 404, true); }
            //Id üzerinden user kontrolü yap
            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if (user == null) { return Response<TokenDto>.Fail("UserId not found", 404, true); }
            //Token oluştur.
            var tokenDto = _tokenService.CreateToken(user);
            //Refresh token güncelle.
            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
            //Kaydet
            await _unitOfWork.CommitAsync();
            //200 ile token dön.
            return Response<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken) //Kullanıcı logout olursa refresh token null olsun.
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh token not found", 404, true);
            }
            _userRefreshTokenService.Remove(existRefreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }
    }
}

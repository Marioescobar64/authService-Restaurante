using System;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Email;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")] // Ruta base: /api/v1/auth
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<object>> GetProfile()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return Unauthorized();
        }

        var user = await authService.GetUserByIdAsync(userIdClaim.Value);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(new
        {
            success = true,
            message = "Perfil obtenido exitosamente",
            data = user
        });
    }

    [HttpPost("profile/by-id")]
    [EnableRateLimiting("ApiPolicy")]
    public async Task<ActionResult<object>> GetProfileById([FromBody] GetProfileByIdDto request)
    {
        if (string.IsNullOrEmpty(request.UserId))
        {
            return BadRequest(new
            {
                success = false,
                message = "El userId es requerido"
            });
        }

        var user = await authService.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Usuario no encontrado"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Perfil obtenido exitosamente",
            data = user
        });
    }

    // <summary>
    // 
    // </summary>
    // <param name="Name"> Nombre del usuario </param>
    // <returns></returns>
    [HttpPost("register")] // Endpoint específico: POST /api/v1/auth/register
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB límite
    [EnableRateLimiting("AuthPolicy")]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromForm] RegisterDto registerDto)
    {
        var result = await authService.RegisterAsync(registerDto);
        // Devolver 201 Created para registro
        return StatusCode(201, result);
    }

    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        return Ok(result);
    }

    [HttpPost("verify-login")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<ActionResult<AuthResponseDto>> VerifyLogin([FromBody] VerifyLoginDto verifyLoginDto)
    {
        var result = await authService.VerifyLoginAsync(verifyLoginDto);
        return Ok(result);
    }

    [HttpPost("verify-email")]
    [EnableRateLimiting("ApiPolicy")]
    public async Task<ActionResult<EmailResponseDto>> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
    {
        var result = await authService.VerifyEmailAsync(verifyEmailDto);
        return Ok(result);
    }

    [HttpGet("verify-email")]
    [EnableRateLimiting("ApiPolicy")]
    public async Task<IActionResult> VerifyEmailHtml([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return Content("<html><head><meta charset=\"UTF-8\"></head><body style='font-family: sans-serif; text-align: center; padding-top: 50px; color: red;'><h2>Error: Token no proporcionado o inválido.</h2></body></html>", "text/html");
        }

        var verifyEmailDto = new VerifyEmailDto { Token = token };
        var result = await authService.VerifyEmailAsync(verifyEmailDto);

        if (result.Success)
        {
            return Content("<html><head><meta charset=\"UTF-8\"></head><body style='font-family: sans-serif; text-align: center; padding-top: 50px;'><h2>¡Correo verificado con éxito!</h2><p>Ya puedes cerrar esta página y volver a la aplicación.</p></body></html>", "text/html");
        }
        
        return Content($"<html><head><meta charset=\"UTF-8\"></head><body style='font-family: sans-serif; text-align: center; padding-top: 50px; color: red;'><h2>Error al verificar el correo</h2><p>{result.Message}</p></body></html>", "text/html");
    }

    [HttpPost("resend-verification")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<ActionResult<EmailResponseDto>> ResendVerification([FromBody] ResendVerificationDto resendDto)
    {
        var result = await authService.ResendVerificationEmailAsync(resendDto);

        if (!result.Success)
        {
            if (result.Message.Contains("no encontrado", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(result);
            }
            if (result.Message.Contains("ya ha sido verificado", StringComparison.OrdinalIgnoreCase) ||
                result.Message.Contains("ya verificado", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(result);
            }
            return StatusCode(503, result);
        }

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<ActionResult<EmailResponseDto>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        var result = await authService.ForgotPasswordAsync(forgotPasswordDto);

        if (!result.Success)
        {
            return StatusCode(503, result);
        }

        return Ok(result);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<ActionResult<EmailResponseDto>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var result = await authService.ResetPasswordAsync(resetPasswordDto);
        return Ok(result);
    }
}
using MovieBookingApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MovieBookingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình DbContext cho PostgreSQL
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Thêm các dịch vụ vào container
            builder.Services.AddControllers();
            
            // Cấu hình CORS để cho phép tất cả các yêu cầu từ các nguồn khác nhau
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    policyBuilder =>
                    {
                        policyBuilder.AllowAnyOrigin()
                                     .AllowAnyMethod()
                                     .AllowAnyHeader();
                    });
            });

            // Thêm cấu hình cho Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
//             builder.Services.AddControllers().AddJsonOptions(options =>
//         {
//            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
// });
            builder.Services.AddHostedService<SeatUnlockService>();

            var app = builder.Build();

            // Cấu hình pipeline HTTP request
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieBookingApp API V1");
                    c.RoutePrefix = string.Empty; // Đặt Swagger UI ở root của ứng dụng
                });
            }

            app.UseHttpsRedirection();

            // Sử dụng CORS
            app.UseCors("AllowAllOrigins");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

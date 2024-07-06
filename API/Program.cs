using System.Buffers;
using System.Text;
using System.Text.Json;
using Application.Commands;
using Application.Extensions;
using Application.Handler;
using Application.Validations;
using DDD_ExceptionHandling.Middleware;
using Domain.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// add logging
builder.Services.AddLogging();

builder.Services.AddTransient<ExceptionMiddleware>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.MapPost("/api/booking", context =>
    {
        var jsonBody = context.Request.BodyReader.ReadAsync().Result.Buffer;
        var json = Encoding.UTF8.GetString(jsonBody.ToArray());
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new DomainValidationException("Invalid request parameters.", new Dictionary<string, string[]>
            {
                {"Body", new[] {"Body is required."}}
            });
        }
        
        var validator = new CreateBookingCommandValidation();
        var createBookingCommand = JsonSerializer.Deserialize<CreateBookingCommand>(json);
        var validationResult = validator.Validate(createBookingCommand);
        validationResult.EnsureValidDomain();
        
        var createBookingHandler = new CreateBookingHandler();
        createBookingHandler.Handle(createBookingCommand);
        
        return Task.CompletedTask;
    });

app.Run();
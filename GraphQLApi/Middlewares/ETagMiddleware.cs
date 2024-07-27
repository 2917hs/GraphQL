namespace GraphQLApi.Middlewares;

public class ETagMiddleware
{
    private readonly RequestDelegate _next;

    public ETagMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            if (context.Response.StatusCode == 200 && context.Items["ETag"] != null)
            {
                context.Response.Headers["ETag"] = context.Items["ETag"].ToString();
            }
            return Task.CompletedTask;
        });

        await _next(context);
    }
}

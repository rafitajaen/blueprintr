namespace Boilerplatr.Abstractions.Results;

public static class ResultExtensions
{
    public static TOutput Match<TOutput>
    (
        this Result result,
        Func<TOutput> onSuccess,
        Func<Result, TOutput> onFailure
    )
    {
        return result.IsSuccess ? onSuccess() : onFailure(result);
    }

    public static TOutput Match<TInput, TOutput>
    (
        this Result<TInput> result,
        Func<TInput, TOutput> onSuccess,
        Func<Result<TInput>, TOutput> onFailure
    )
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
    }
}

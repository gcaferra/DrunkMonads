namespace Monads.Demo;
using static MonadsFuncExtensions;

public class MonadsFunc
{
    Func<string,int> lenght = text => text.Length;

    Func<int, double> twice = i => i * 2;

    private Func<string?, MaybeU<int>> maybeLength = text =>
        text != null
            ? just(text.Length)
            : nothing<int>();

    private Func<int, MaybeU<double>> twiceNoNegative = i
        => i > 0
            ? just(i * 2d)
            : nothing<double>();
    
    void Main()
    {
        // var comp = MonadsFuncExtensions.compose(twice, lenght);
        var comp = twice
            .compose(lenght);
    }

    void Maybe()
    {
        // (b -> c) -> (a -> b) -> (a -> c) compose
        // (b -> Maybe c) -> (a -> Maybe b) -> (a -> Maybe c) compose
        Func<MaybeU<int>> stupidSmart = () => @return(42);
    }
}
// Type Maybe(Type a) => 
// data Maybe a = Just a | Nothing
internal abstract record MaybeU<T>;

internal record Just<T>(T Value) : MaybeU<T>;
internal record Nothing<T>() : MaybeU<T>;

internal static class MonadsFuncExtensions
{
    // internal static B bind<A, B>(this Func<A, B> f, A a) => f(a);
    internal static B bind<A, B>(this Func<A, B> f, A a) => f(a);
    internal static MaybeU<B> bind<A,B>(this Func<A, MaybeU<B>> f, MaybeU<A> maybeA)
    {
        return matchAndMap(
            maybeA,
            a => f(a),
            () => nothing<B>());
    }

    internal static Func<A, C> compose<A, B, C>(this Func<B, C> f, Func<A, B> g)
        => a => f(g(a));
    
    // (a -> Maybe b) -> Maybe a -> Maybe b
    // (a -> Maybe b) -> (Maybe a -> Maybe b)
    // (b -> Maybe c) -> (a -> Maybe b) -> a -> Maybe c
    internal static Func<A,MaybeU<C>> composeU<A,B,C>
        (Func<B, MaybeU<C>> f,Func<A, MaybeU<B>> g) =>
        a => bind(f, g(a));
   //  v >>= f >>= g >>= r >>=
   // f . g . z . 
    // (b -> Maybe c) -> (a -> Maybe b) -> a -> Maybe c
    internal static Func<A,MaybeU<C>> compose<A,B,C>
        (Func<B, MaybeU<C>> f,Func<A, MaybeU<B>> g) =>
        a => matchAndMap(
            g(a), 
            f,
            () => new Nothing<C>());

    internal static MaybeU<T> just<T>(T t) => new Just<T>(t);
    internal static MaybeU<T> @return<T>(T t) => just(t);
    internal static MaybeU<T> nothing<T>() => new Nothing<T>();
    
    internal static B matchAndMap<A,B>(MaybeU<A> value, Func<A,B> then, Func<B> @else)
    {
        return value switch
        {
            Just<A> just => then(just.Value),
            Nothing<A> nothing => @else(),
        };
    }
}
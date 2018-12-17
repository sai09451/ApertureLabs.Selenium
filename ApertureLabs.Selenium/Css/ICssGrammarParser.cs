namespace ApertureLabs.Selenium.Css
{
    /// <summary>
    /// Used to parse css strings.
    /// </summary>
    /// <seealso cref="https://drafts.csswg.org/css-syntax-3/#parse-comma-separated-list-of-component-values"/>
    public interface ICssGrammarParser
    {
        bool TryParse(string cssString, params ICssGrammar[] parsers);
    }
}

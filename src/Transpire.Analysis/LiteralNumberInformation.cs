using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics.CodeAnalysis;

namespace Transpire.Analysis;

internal sealed class LiteralNumberInformation
{
	[SetsRequiredMembers]
	internal LiteralNumberInformation(LiteralExpressionSyntax syntax)
	{
		static string GetTypeSuffix(string literalText, bool isHexLiteral)
		{
			static bool IsTypeSuffixCharacter(char typeSuffixCandidate, bool isHexLiteral) =>
				(isHexLiteral && !Uri.IsHexDigit(typeSuffixCandidate)) ||
				!char.IsDigit(typeSuffixCandidate);

			var typeSuffix = literalText[literalText.Length - 1];

			if (IsTypeSuffixCharacter(typeSuffix, isHexLiteral))
			{
				var secondTypeSuffix = literalText[literalText.Length - 2];

				return IsTypeSuffixCharacter(secondTypeSuffix, isHexLiteral) ?
					$"{secondTypeSuffix}{typeSuffix}" : $"{typeSuffix}";
			}

			return string.Empty;
		}

		this.Prefix = this.WholePart =
			this.DecimalPoint = this.FractionalPart = this.Exponent =
			this.TypeSuffix = string.Empty;

		var literalText = syntax.Token.Text;

		if (literalText.Length >= 2)
		{
			const int PrefixOffset = 2;

			var prefix = literalText.Substring(0, PrefixOffset);

			if (prefix == "0x" || prefix == "0X" || prefix == "0b" || prefix == "0B")
			{
				this.Prefix = prefix;

				// We know at this point this is an integer, so the only thing we need to grab
				// is the whole part and a type suffix if it exists.
				this.TypeSuffix = GetTypeSuffix(literalText, true);
				this.WholePart = literalText.Substring(
					PrefixOffset, literalText.Length - PrefixOffset - this.TypeSuffix.Length);
			}
			else
			{
				// At this point, we don't know if it's a integer or floating point number.
				// We could have "3d" as the literal, so we have to be a bit careful.
				// First, look at the last character(s) to determine if it has a type suffix.
				this.TypeSuffix = GetTypeSuffix(literalText, false);
				var remainderPart = literalText.Substring(0, literalText.Length - this.TypeSuffix.Length);
				// Look for the exponent if it exists.
				var exponentPosition = remainderPart.IndexOf('e');

				if (exponentPosition > 0)
				{
					this.Exponent = remainderPart.Substring(exponentPosition);
					remainderPart = remainderPart.Substring(0, exponentPosition);
				}

				var dotPosition = remainderPart.IndexOf('.');

				if (dotPosition > -1)
				{
					this.DecimalPoint = ".";
					var parts = remainderPart.Split('.');
					this.WholePart = parts[0];
					this.FractionalPart = this.Exponent == string.Empty ?
						parts[1] : parts[1].Replace(this.Exponent, string.Empty);
				}
				else
				{
					this.WholePart = this.Exponent == string.Empty ?
						remainderPart : remainderPart.Replace(this.Exponent, string.Empty);
				}
			}
		}
		else
		{
			this.WholePart = literalText;
		}

		this.NeedsSeparators = this.Prefix.Length > 0 ?
			this.WholePart.Length > 2 && !this.WholePart.Contains('_') :
			((this.WholePart.Length > 3 && !this.WholePart.Contains('_')) ||
			(this.FractionalPart.Length > 3 && !this.FractionalPart.Contains('_')));
	}

	[SetsRequiredMembers]
	private LiteralNumberInformation(
		string prefix, string wholePart,
		string decimalPoint, string fractionalPart, string exponent,
		string typeSuffix, bool needsSeparator) =>
		(this.Prefix, this.WholePart, this.DecimalPoint,
		this.FractionalPart, this.Exponent, this.TypeSuffix, this.NeedsSeparators) =
		(prefix, wholePart, decimalPoint,
		fractionalPart, exponent, typeSuffix, needsSeparator);

	internal LiteralNumberInformation CreateSeparated(uint spacingSize)
	{
		if (!this.NeedsSeparators)
		{
			return this;
		}

		// TODO: This needs to actually change WholePart and/or FractionalPart,
		// based on spacingSize.
		return new(this.Prefix, this.WholePart, this.DecimalPoint, this.FractionalPart, this.Exponent, this.TypeSuffix, false);
	}

	public override string ToString() =>
		$"{this.Prefix}{this.WholePart}{this.DecimalPoint}{this.FractionalPart}{this.Exponent}{this.TypeSuffix}";

	/// <summary>
	/// Gets the decimal point.
	/// Either "." or "".
	/// </summary>
	internal required string DecimalPoint { get; init; }

	/// <summary>
	/// Gets the exponent.
	/// For floating points, something like "e2" (if it exists).
	/// </summary>
	internal required string Exponent { get; init; }

	/// <summary>
	/// Gets the fractional part.
	/// For "123", this would be "". "123.45" would give "45".
	/// </summary>
	internal required string FractionalPart { get; init; }

	/// <summary>
	/// Gets whether a separator is needed or not.
	/// Returns <see langword="true"/> if <see cref="WholePart"/> or <see cref="FractionalPart"/> should have a separator in them.
	/// </summary>
	internal required bool NeedsSeparators { get; init; }

	/// <summary>
	/// Gets the prefix.
	/// Either "0x/X" or "0b/B" or "".
	/// </summary>
	internal required string Prefix { get; init; }

	/// <summary>
	/// Gets the type suffix.
	/// Can be "U" or "UL" or "d" or "".
	/// </summary>
	internal required string TypeSuffix { get; init; }

	/// <summary>
	/// Gets the whole part.
	/// For "123", this would be "123". "123.45" would give "123".
	/// </summary>
	internal required string WholePart { get; init; }
}
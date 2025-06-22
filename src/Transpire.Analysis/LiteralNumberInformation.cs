using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics.CodeAnalysis;

namespace Transpire.Analysis;

internal sealed class LiteralNumberInformation
{
	[SetsRequiredMembers]
	internal LiteralNumberInformation(LiteralExpressionSyntax syntax)
	{
		this.Prefix = this.WholePart =
			this.DecimalPoint = this.FractionalPart = this.Exponent =
			this.TypeSuffix = string.Empty;

		var literalText = syntax.Token.Text;
		var offset = 0;

		if (literalText.Length >= 2 + offset)
		{
			var prefix = literalText.Substring(offset, 2);

			if (prefix == "0x" || prefix == "0X" || prefix == "0b" || prefix == "0B")
			{
				this.Prefix = prefix;
				offset += 2;

				// We know at this point this is an integer, so the only thing we need to grab
				// is the whole part and a type suffix if it exists.
				var literalTextRemainderSpan = literalText.AsSpan(offset);
				var typeSuffix = literalTextRemainderSpan[literalTextRemainderSpan.Length - 1];

				if (!Uri.IsHexDigit(typeSuffix))
				{
					var secondTypeSuffix = literalTextRemainderSpan[literalTextRemainderSpan.Length - 2];

					this.TypeSuffix = Uri.IsHexDigit(secondTypeSuffix) ?
						$"{secondTypeSuffix}{typeSuffix}" : $"{typeSuffix}";
				}

				this.WholePart = literalText.Substring(offset, literalText.Length - offset - this.TypeSuffix.Length);
			}
			else
			{
				// At this point, we don't know if it's a integer or floating point number.
				// First, look at the last character(s) to determine if it has a type suffix.
			}
		}
	}

	[SetsRequiredMembers]
	internal LiteralNumberInformation(
		string prefix, string wholePart,
		string decimalPoint, string fractionalPart, string exponent,
		string typeSuffix, bool needsSeparator) =>
		(this.Prefix, this.WholePart, this.DecimalPoint,
		this.FractionalPart, this.Exponent, this.TypeSuffix, this.NeedsSeparators) =
		(prefix, wholePart, decimalPoint,
		fractionalPart, exponent, typeSuffix, needsSeparator);

	public void Deconstruct(out string prefix, out string wholePart,
		out string decimalPoint, out string fractionalPart, out string exponent,
		out string typeSuffix, out bool needsSeparator) =>
		(prefix, wholePart, decimalPoint,
		fractionalPart, exponent, typeSuffix, needsSeparator) =
		(this.Prefix, this.WholePart, this.DecimalPoint,
		this.FractionalPart, this.Exponent, this.TypeSuffix, this.NeedsSeparators);

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
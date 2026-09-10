using System;

using Skyline.DataMiner.Analytics.GenericInterface;

namespace SLCGQIOCombineColumns
{
    /// <summary>
    /// Combines two columns of the same supported type into a new column.
    /// </summary>
    [GQIMetaData(Name = "Combine Columns")]
    public sealed class SLCGQIOCombineColumns : IGQIInputArguments, IGQIColumnOperator, IGQIRowOperator
    {
        private readonly GQIColumnDropdownArgument firstColumnArgument = new GQIColumnDropdownArgument("First column")
        {
            IsRequired = true,
            Types = new[]
            {
                GQIColumnType.String,
                GQIColumnType.Int,
                GQIColumnType.DateTime,
                GQIColumnType.Boolean,
                GQIColumnType.Double,
                GQIColumnType.TimeSpan,
            },
        };

        private readonly GQIColumnDropdownArgument secondColumnArgument = new GQIColumnDropdownArgument("Second column")
        {
            IsRequired = true,
            Types = new[]
            {
                GQIColumnType.String,
                GQIColumnType.Int,
                GQIColumnType.DateTime,
                GQIColumnType.Boolean,
                GQIColumnType.Double,
                GQIColumnType.TimeSpan,
            },
        };

        private GQIColumn firstColumn;
        private GQIColumn secondColumn;
        private GQIColumn outputColumn;

        /// <summary>
        /// Gets the columns that must be selected for the operation.
        /// </summary>
        /// <returns>The required input arguments.</returns>
        public GQIArgument[] GetInputArguments()
        {
            return new GQIArgument[] { firstColumnArgument, secondColumnArgument };
        }

        /// <summary>
        /// Processes and validates the selected columns.
        /// </summary>
        /// <param name="args">The processed input arguments.</param>
        /// <returns>The argument processing result.</returns>
        /// <exception cref="GenIfException">Thrown when the selected columns have different types.</exception>
        public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
        {
            firstColumn = args.GetArgumentValue(firstColumnArgument);
            secondColumn = args.GetArgumentValue(secondColumnArgument);

            if (firstColumn.Type != secondColumn.Type)
            {
                throw new GenIfException("The first and second columns must have the same type.");
            }

            outputColumn = CreateOutputColumn(firstColumn);
            return new OnArgumentsProcessedOutputArgs();
        }

        /// <summary>
        /// Adds the combined column to the query result.
        /// </summary>
        /// <param name="header">The editable query header.</param>
        public void HandleColumns(GQIEditableHeader header)
        {
            header.AddColumns(new[] { outputColumn });
        }

        /// <summary>
        /// Sets the combined value for a row.
        /// </summary>
        /// <param name="row">The editable query row.</param>
        public void HandleRow(GQIEditableRow row)
        {
            switch (firstColumn.Type)
            {
                case GQIColumnType.String:
                    SetStringValue(row);
                    break;
                case GQIColumnType.Int:
                    SetValue<int>(row);
                    break;
                case GQIColumnType.Boolean:
                    SetValue<bool>(row);
                    break;
                case GQIColumnType.Double:
                    SetValue<double>(row);
                    break;
                case GQIColumnType.DateTime:
                    SetValue<DateTime>(row);
                    break;
                case GQIColumnType.TimeSpan:
                    SetValue<TimeSpan>(row);
                    break;
                default:
                    throw new GenIfException($"Column type '{firstColumn.Type}' is not supported.");
            }
        }

        private static GQIColumn CreateOutputColumn(GQIColumn sourceColumn)
        {
            string name = $"{sourceColumn.Name} (combined)";

            switch (sourceColumn.Type)
            {
                case GQIColumnType.String:
                    return new GQIStringColumn(name);
                case GQIColumnType.Int:
                    return new GQIIntColumn(name);
                case GQIColumnType.Boolean:
                    return new GQIBooleanColumn(name);
                case GQIColumnType.Double:
                    return new GQIDoubleColumn(name);
                case GQIColumnType.DateTime:
                    return new GQIDateTimeColumn(name);
                case GQIColumnType.TimeSpan:
                    return new GQITimeSpanColumn(name);
                default:
                    throw new GenIfException($"Column type '{sourceColumn.Type}' is not supported.");
            }
        }

        private void SetStringValue(GQIEditableRow row)
        {
            if (row.TryGetValue(firstColumn, out string firstValue) && !String.IsNullOrEmpty(firstValue))
            {
                row.SetValue(outputColumn, firstValue);
                return;
            }

            if (row.TryGetValue(secondColumn, out string secondValue) && !String.IsNullOrEmpty(secondValue))
            {
                row.SetValue(outputColumn, secondValue);
            }
        }

        private void SetValue<T>(GQIEditableRow row)
        {
            if (row.TryGetValue(firstColumn, out T firstValue))
            {
                row.SetValue(outputColumn, firstValue);
                return;
            }

            if (row.TryGetValue(secondColumn, out T secondValue))
            {
                row.SetValue(outputColumn, secondValue);
            }
        }
    }
}

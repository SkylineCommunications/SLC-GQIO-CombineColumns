# SLC-GQIO-CombineColumns

## About

SLC-GQIO-CombineColumns is a GQI data transformer that combines two columns into one typed result column. It uses the first column value when available and falls back to the second column value.

## Key Features

- Supports columns of type string, integer, double, boolean, datetime, and timespan.
- Requires both selected columns to have the same type.
- Preserves valid default values, including zero, `false`, `DateTime.MinValue`, and `TimeSpan.Zero`.
- Names the result `<First column name> (combined)`.

## Use Cases

Use this data transformer when equivalent data can be provided by either of two query columns, but consumers need one consistent result column.

## Prerequisites

- DataMiner 10.4.0.0-14003 or later.

## Technical Reference

Select *First column* and *Second column* in the GQI operator configuration. For each row, the operator applies the following rule:

1. If the first column contains a value, that value is used.
1. Otherwise, if the second column contains a value, that value is used.
1. If neither column contains a value, the result remains empty.

For string columns, `null` and an empty string are treated as empty. Whitespace is treated as a value.

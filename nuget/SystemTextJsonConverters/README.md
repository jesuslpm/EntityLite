# inercya.System.Text.Json.Converters

This package contains three custom converters for System.Text.Json:

## RoundDateJsonConverter

It is a custom converter for DateTime type that rounds the date to the nearest day. 
It is useful when you want to serialize a DateTime object to JSON and you want to ignore the time part of the date.

It also rounds the date to the nearest day when deserializing a JSON string to a DateTime object.

## UtcDateTimeJsonConverter

It is a custom converter for DateTime type that converts the date to UTC format when serializing to JSON.
If the date is already UTC, it will not be modified.
If the date is in local time, it will be converted to UTC by calling DateTime.ToUniversalTime().
If the date is in unspecified time, it will be treated as it was in UTC by calling DateTime.SpecifyKind(date, DateTimeKind.Utc).

When deserializing a JSON string to a DateTime object, it will convert the date to UTC format if it is not already in UTC format.

It is useful when you store UTC dates in your database and you want to serialize them to JSON in UTC format. 
Because when you read dates from the database, they are unspecified.

## LocalDateTimeJsonConverter

It is a custom converter for DateTime type that converts the date to local time when serializing to JSON.
If the date is already local, it will not be modified.
If the date is UTC, it will be converted to local by calling DateTime.ToLocalTime().
If the date is in unspecified time, it will be treated as it was in local time by calling DateTime.SpecifyKind(date, DateTimeKind.Local).

When deserializing a JSON string to a DateTime object, it will convert the date to local if it is not already in UTC format.

It is useful when you store local time dates in your database and you want to serialize them to JSON with the offset. 
Because when you read dates from the database, they are unspecified.




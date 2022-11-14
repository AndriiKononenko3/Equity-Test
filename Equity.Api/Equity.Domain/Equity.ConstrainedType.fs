namespace Equity.Domain

open System

/// Useful functions for constrained types
module ConstrainedType =

    /// Create a constrained string using the constructor provided
    /// Return Error if input is null, empty, or length > maxLen
    let createString fieldName ctor maxLen str = 
        if String.IsNullOrEmpty(str) then
            let msg = $"%s{fieldName} must not be null or empty" 
            Error msg
        elif str.Length > maxLen then
            let msg = $"%s{fieldName} must not be more than %i{maxLen} chars" 
            Error msg 
        else
            Ok (ctor str)

    /// Create a optional constrained string using the constructor provided
    /// Return None if input is null, empty. 
    /// Return error if length > maxLen
    /// Return Some if the input is valid
    let createStringOption fieldName ctor maxLen str = 
        if String.IsNullOrEmpty(str) then
            Ok None
        elif str.Length > maxLen then
            let msg = $"%s{fieldName} must not be more than %i{maxLen} chars" 
            Error msg 
        else
            Ok (ctor str |> Some)

    /// Create a constrained integer using the constructor provided
    /// Return Error if input is less than minVal or more than maxVal
    let createInt fieldName ctor minVal maxVal i = 
        if i < minVal then
            let msg = $"%s{fieldName}: Must not be less than %i{minVal}"
            Error msg
        elif i > maxVal then
            let msg = $"%s{fieldName}: Must not be greater than %i{maxVal}"
            Error msg
        else
            Ok (ctor i)

    /// Create a constrained decimal using the constructor provided
    /// Return Error if input is less than minVal or more than maxVal
    let createDecimal fieldName ctor minVal maxVal i = 
        if i < minVal then
            let msg = DecimalValueExceedsLimit $"%s{fieldName}: Must not be less than %M{minVal}"
            Error msg
        elif i > maxVal then
            let msg = DecimalValueExceedsLimit $"%s{fieldName}: Must not be greater than %M{maxVal}"
            Error msg
        else
            Ok (ctor i)

    /// Create a constrained string using the constructor provided
    /// Return Error if input is null. empty, or does not match the regex pattern
    let createLike fieldName  ctor pattern str = 
        if String.IsNullOrEmpty(str) then
            let msg = $"%s{fieldName}: Must not be null or empty" 
            Error msg
        elif System.Text.RegularExpressions.Regex.IsMatch(str,pattern) then
            Ok (ctor str)
        else
            let msg = $"%s{fieldName}: '%s{str}' must match the pattern '%s{pattern}'"
            Error msg
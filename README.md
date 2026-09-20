# FunctionalTicketCalculator

## Run
```bash
dotnet run --project FunctionalTicketCalculator
dotnet test
```

## Questions

### 1. Which parts of the program are imperative?
`Main` and `ProcessInput`. They handle console I/O, mutable parsing state, and sequential execution.

### 2. Which functions are pure?
`CalculateFinalPrice`, `ApplyPricingRule`, and `GetCategoryDiscountRule`. They are deterministic and have no side effects.

### 3. Where do side effects remain?
Only in `Console.ReadLine()` and `Console.WriteLine()` at the application entry boundary.

### 4. Why is TryParse preferred to Parse for user input?
`TryParse` returns a boolean status without throwing expensive exceptions on invalid input.

## Test Cases

| Price | Age | Student | Ticket | Day | Expected |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 5000 | 5 | false | Standard | Weekday | 0.00 |
| 5000 | 10 | false | Standard | Weekday | 2500.00 |
| 5000 | 20 | true | Standard | Weekday | 4250.00 |
| 5000 | 65 | false | Standard | Weekday | 3500.00 |
| 5000 | 30 | false | Vip | Weekday | 6250.00 |
| 5000 | 30 | false | Standard | Weekend | 5500.00 |
| 5000 | 20 | true | Vip | Weekend | 5843.75 |
| -100 | 20 | false | Standard | Weekday | Error |
| 5000 | abc | false | Standard | Weekday | Error |

# FunctionalTicketCalculator

Console-based ticket price calculator implemented in C# (.NET 9) with functional programming principles: pure calculation core, higher-order functions, and an imperative I/O shell.

## Build and Run

### Run the application
Interactive mode:
```bash
dotnet run --project FunctionalTicketCalculator
```

Passing parameters via CLI arguments (`<price> <age> <isStudent> <ticketType> <dayType>`):
```bash
dotnet run --project FunctionalTicketCalculator -- 5000 20 true Vip Weekend
```

### Run tests
```bash
dotnet test
```

---

## Technical Questions & Answers

### 1. Which parts of the program are imperative?
The imperative parts are located exclusively in the I/O boundary: `Main` and `ProcessInput`.
- Reading input strings from `Console.ReadLine()`
- Parsing and mutating control state via `TryParse(..., out ...)`
- Sequential guard clauses with early returns (`if (...) return;`)
- Writing messages and formatted results to `Console.WriteLine()`

These parts control program execution order, manage mutable state (output parameters), and perform I/O operations.

### 2. Which functions are pure?
The functions responsible for pricing logic are pure:
- `ApplyPricingRule(decimal price, Func<decimal, decimal> rule)`
- `GetCategoryDiscountRule(int age, bool isStudent)`
- `CalculateFinalPrice(decimal basePrice, int age, bool isStudent, TicketType ticketType, DayType dayType)`

They satisfy the definitions of purity:
1. **Deterministic / Referentially Transparent**: Given identical input parameters, they always return the exact same value.
2. **No Side Effects**: They do not read or write to the console, mutate global state, or modify their inputs.

### 3. Where do side effects remain?
Side effects are strictly isolated to the outer shell:
- `Console.ReadLine()` reads external state from standard input.
- `Console.WriteLine()` modifies external state by writing to standard output.

By separating the pure computation (`CalculateFinalPrice`) from the I/O boundary (`ProcessInput` / `Main`), the business logic remains fully testable without mocking console streams.

### 4. Why is TryParse preferred to Parse for user input?
- `Parse` throws an exception (e.g., `FormatException`, `OverflowException`, `ArgumentNullException`) when encountering malformed input. Using exceptions for regular control flow is anti-pattern in high-performance C# because exception generation unwinds the stack and creates significant CPU/memory overhead.
- `TryParse` follows the Tester-Doer pattern: it returns a `bool` indicating success or failure and writes the result to an `out` parameter. This enables safe early returns without crashing the application or paying the performance cost of exceptions.

---

## Functional Requirements Implementation

- **Named static methods**: `ApplyPricingRule`, `GetCategoryDiscountRule`, `CalculateFinalPrice`.
- **Higher-order function**: `ApplyPricingRule(decimal price, Func<decimal, decimal> rule) => rule(price);` (accepts a function delegate as a parameter).
- **Func<...> values**: `categoryRule`, `vipRule`, and `weekendRule` are stored in variables of type `Func<decimal, decimal>`.
- **Lambda expressions**: Modifiers like `p => p * 1.25m` and `p => p * 1.10m` are expressed as lambda functions.
- **Rule priority**: Customer categories are evaluated in exact priority order (younger than 6 -> 6 to 12 -> student -> 60+ -> other). If a customer matches multiple categories (e.g., student aged 65), the student discount applies first.

---

## Documented Test Cases

| Base Price | Age | Student | Ticket Type | Day Type | Expected Price | Description |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| 5000 | 5 | false | Standard | Weekday | 0.00 | Child under 6: 100% discount (free) |
| 5000 | 10 | false | Standard | Weekday | 2500.00 | Child 6–12: 50% discount |
| 5000 | 20 | true | Standard | Weekday | 4250.00 | Student: 15% discount |
| 5000 | 65 | false | Standard | Weekday | 3500.00 | Senior (60+): 30% discount |
| 5000 | 30 | false | Vip | Weekday | 6250.00 | Adult + VIP (+25%) |
| 5000 | 30 | false | Standard | Weekend | 5500.00 | Adult + Weekend (+10%) |
| 5000 | 20 | true | Vip | Weekend | 5843.75 | Student (-15%) + VIP (+25%) + Weekend (+10%) |
| 5000 | -1 | false | Standard | Weekday | Error | Negative age validation |
| -100 | 20 | false | Standard | Weekday | Error | Negative price validation |
| 5000 | abc | false | Standard | Weekday | Error | Non-numeric age validation |
| 5000 | 20 | maybe | Standard | Weekday | Error | Non-boolean student status |
| 5000 | 20 | false | Premium | Weekday | Error | Unknown ticket type |
| 0 | 25 | false | Standard | Weekday | 0.00 | Zero base price |

# Phone Code Reference Guide

## Common Country Phone Codes

When registering users, use these phone codes (without the + symbol):

### Africa
- **Nigeria**: `234`
- **South Africa**: `27`
- **Kenya**: `254`
- **Ghana**: `233`
- **Egypt**: `20`
- **Ethiopia**: `251`

### Europe
- **United Kingdom**: `44`
- **France**: `33`
- **Germany**: `49`
- **Italy**: `39`
- **Spain**: `34`
- **Netherlands**: `31`

### North America
- **United States**: `1`
- **Canada**: `1`
- **Mexico**: `52`

### Asia
- **China**: `86`
- **India**: `91`
- **Japan**: `81`
- **South Korea**: `82`
- **Singapore**: `65`
- **UAE**: `971`
- **Saudi Arabia**: `966`

### Oceania
- **Australia**: `61`
- **New Zealand**: `64`

## Usage Examples

### Nigerian Number
```json
{
  "phoneCode": "234",
  "phoneNumber": "8012345678"
}
```
Full number: +234 801 234 5678

### UK Number
```json
{
  "phoneCode": "44",
  "phoneNumber": "7700900123"
}
```
Full number: +44 7700 900123

### US Number
```json
{
  "phoneCode": "1",
  "phoneNumber": "2025551234"
}
```
Full number: +1 202 555 1234

## Validation Rules

- **phoneCode**: 
  - Optional field
  - Max 5 characters
  - Digits only (no + symbol)
  
- **phoneNumber**: 
  - Optional field
  - Max 15 characters
  - Digits only (no spaces or special characters)

## Notes

- Both fields are optional, but if you provide a phone number, it's recommended to also provide the phone code
- Store numbers without formatting (spaces, dashes, parentheses)
- The maximum length for phone codes is 5 digits to accommodate all international codes
- Phone numbers are stored without the leading zero (for countries like Nigeria, UK, etc.)

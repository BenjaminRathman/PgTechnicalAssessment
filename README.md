# Prometheus Group - Technical Assessment

This is Benjamin Rathman's solution to the Prometheus Group's Technical Assessment

This project uses .NET 8 and a Alpha Vantage free API key that
- Calls the Alpha Vantage `TIME_SERIES_INTRADAY` endpoint
- Aggregates the last 30 days of 15-minute intraday data
- Exposes an new API returns this data 

## To Run 
- Clone the repo

- Get a free API Key at https://www.alphavantage.co/documentation/ 

- Add this key in the User's Secrets (dotnet user-secrets set "AlphaVantageApiKey": "KEY")

- Build the solution and swagger should come up to test the exposed API

## Assumptions I Made 

- I assumed that Alpha Vantage always should be returning something (if not we throw an exception). 

- I also assumed based on the instruction "queries the intraday data for last month" I needed to sort out any extra data that AlphaVantage may have returned passed 30 days (even with outputsize=full Alpha vantage can return days past a month because the days that it returns are trailing)

- I would be happy to discuss how I would make changes if these assumptions are incorrect

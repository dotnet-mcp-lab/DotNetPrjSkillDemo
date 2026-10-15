# Json Data Azure Search Indexer

1. Upload the json file in JsonData/customer-orders folder to Azure Blob Storage, as container name is `customer-orders`.
2. Set various User Secrets.
   ```sh
   dotnet user-secrets set "SearchServiceEndpoint" "https://YOUR-SEARCH-SERVICE.search.windows.net"
   dotnet user-secrets set "SearchServiceAdminKey" "YOUR-SEARCH-ADMIN-KEY"
   dotnet user-secrets set "BlobConnectionString" "YOUR-BLOB-CONNECTION-STRING"
   ```
3. Run `dotnet run` to start the indexer.
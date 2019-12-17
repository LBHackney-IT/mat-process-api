FROM microsoft/dotnet:2.2-sdk

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./mat-process-api.sln ./
COPY ./mat-process-api/mat-process-api.csproj ./mat-process-api/
COPY ./mat-process-api.Tests/mat-process-api.Tests.csproj ./mat-process-api.Tests/

RUN dotnet restore ./mat-process-api/mat-process-api.csproj
RUN dotnet restore ./mat-process-api.Tests/mat-process-api.Tests.csproj

# Copy everything else and build
COPY . .
COPY ./mat-process-api/rds-combined-ca-bundle.pem /tmp/rds-combined-ca-bundle.pem
RUN dotnet build -c debug -o out mat-process-api.Tests/mat-process-api.Tests.csproj

CMD dotnet test

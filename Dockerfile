# === Stage 1: Build এনভায়রনমেন্ট ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ক্যাশিং অপ্টিমাইজ করার জন্য প্রথমে শুধু .csproj ফাইল কপি করে রিস্টোর করা
COPY ["Boi.Net/Boi.Net.csproj", "./Boi.Net/"]
RUN dotnet restore "Boi.Net/Boi.Net.csproj"

# প্রজেক্ট ফাইল এবং প্রপার্টিজ কপি করা
COPY ["Boi.Net/", "./Boi.Net/"]

# কন্ট্রোলার, সার্ভিস, মডেল এবং ডিটিও ফাইল কপি করা
COPY ["Boi.Net/Controllers/", "./Boi.Net/Controllers/"]
COPY ["Boi.Net/Services/", "./Boi.Net/Services/"]
COPY ["Boi.Net/Model/", "./Boi.Net/Model/"]
COPY ["Boi.Net/DTOs/", "./Boi.Net/DTOs/"]
COPY ["Boi.Net/Data/", "./Boi.Net/Data/"]
COPY ["Boi.Net/Mappers/", "./Boi.Net/Mappers/"]
COPY ["Boi.Net/Exceptions/", "./Boi.Net/Exceptions/"]
COPY ["Boi.Net/Settings/", "./Boi.Net/Settings/"]
COPY ["Boi.Net/Migrations/", "./Boi.Net/Migrations/"]
COPY ["Boi.Net/Properties/", "./Boi.Net/Properties/"]

# এপিসেটিংস ফাইল কপি করা (Production এবং Development উভয়ই)
COPY ["Boi.Net/appsettings.json", "./Boi.Net/"]
COPY ["Boi.Net/appsettings.Development.json", "./Boi.Net/"]

# প্রজেক্ট বিল্ড এবং পাবলিশ করা
RUN dotnet publish "Boi.Net/Boi.Net.csproj" -c Release -o /app/publish /p:UseAppHost=false

# === Stage 2: Runtime এনভায়রনমেন্ট (ফাইনাল ইমেজ) ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# মেটাডেটা
LABEL maintainer="Boi.Net Development Team"
LABEL description="Boi.Net - Bookstore E-commerce Platform API"

WORKDIR /app

# Build স্টেজ থেকে published আউটপুট কপি করা
COPY --from=build /app/publish .

# এনভায়রনমেন্ট ভেরিয়েবল সেট করা
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Render এর জন্য পোর্ট এক্সপোজ করা
EXPOSE 8080

# হেলথ চেক (অপশনাল কিন্তু সুপারিশকৃত)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://localhost:8080/swagger/ || exit 1

# অ্যাপ্লিকেশন রান করার কমান্ড
ENTRYPOINT ["dotnet", "Boi.Net.dll"]
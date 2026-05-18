# === Stage 1: Build এনভায়রনমেন্ট ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ক্যাশিং অপ্টিমাইজ করার জন্য প্রথমে শুধু .csproj ফাইল কপি করে রিস্টোর করা
COPY ["Boi.Net/Boi.Net.csproj", "Boi.Net/"]
RUN dotnet restore "Boi.Net/Boi.Net.csproj"

# গিটহাব থেকে বাকি সব সোর্স কোড (Controllers, Services, Models ইত্যাদি) একবারে কপি করা
COPY . .

# প্রজেক্ট বিল্ড এবং পাবলিশ করা
RUN dotnet publish "Boi.Net/Boi.Net.csproj" -c Release -o /app/publish /p:UseAppHost=false

# === Stage 2: Runtime এনভায়রনমেন্ট (ফাইনাল ইমেজ) ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

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

# অ্যাপ্লিকেশন রান করার কমান্ড
ENTRYPOINT ["dotnet", "Boi.Net.dll"]
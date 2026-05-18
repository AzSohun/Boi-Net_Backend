# === Stage 1: Build এনভায়রনমেন্ট ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ক্যাশিং অপ্টিমাইজ করার জন্য প্রথমে শুধু .csproj ফাইল কপি করে রিস্টোর করা
COPY ["Boi.Net.csproj", "./"]
RUN dotnet restore "Boi.Net.csproj"

# বাকি সব সোর্স কোড কপি করা
COPY . .

# প্রজেক্ট বিল্ড এবং পাবলিশ করা
RUN dotnet publish "Boi.Net.csproj" -c Release -o /app/publish /p:UseAppHost=false

# === Stage 2: Runtime এনভায়রনমেন্ট (ফাইনাল ইমেজ) ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render ডিফল্টভাবে পোর্ট পোর্ট ডিটেক্ট করতে পারে, তবে .NET কে নির্দিষ্ট করে দেওয়ার জন্য:
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# অ্যাপ্লিকেশন রান করার কমান্ড
ENTRYPOINT ["dotnet", "Boi.Net.dll"]
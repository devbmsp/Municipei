using FirebaseAdmin;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Municipei.Interface;
using Municipei.Service;

var builder = WebApplication.CreateBuilder(args);
GoogleCredential credential = GoogleCredential.FromFile("/Users/bm/Projects/Municipei/Municipei/Data/municipeibd-firebase-adminsdk-fbsvc-85a7e4a7f5.json");
FirebaseApp.Create(new AppOptions()
{
    Credential = credential
});
var firestoreClientBuilder = new FirestoreClientBuilder
{
    Credential = credential
};
FirestoreClient firestoreClient = firestoreClientBuilder.Build();

// 4. Cria o FirestoreDb usando o FirestoreClient
FirestoreDb data = FirestoreDb.Create("municipeibd", firestoreClient);
builder.Services.AddSingleton(data);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IHomeModel, HomeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


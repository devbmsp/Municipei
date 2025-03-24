using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Municipei.Interface;
using Municipei.Service;

var builder = WebApplication.CreateBuilder(args);

GoogleCredential credential = GoogleCredential.FromFile("wwwroot/Data/municipeibd-firebase-adminsdk-fbsvc-85a7e4a7f5.json");
FirebaseApp.Create(new AppOptions
{
    Credential = credential
});
builder.Services.AddSingleton<GoogleDriveService>(provider =>
{
    return new GoogleDriveService("wwwroot/Data/municipeibd-firebase-adminsdk-fbsvc-85a7e4a7f5.json");
});

var firestoreClientBuilder = new FirestoreClientBuilder
{
    Credential = credential
};
FirestoreClient firestoreClient = firestoreClientBuilder.Build();

FirestoreDb data = FirestoreDb.Create("municipeibd", firestoreClient);

builder.Services.AddSingleton(data);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IHomeModel, HomeService>();
builder.Services.AddScoped<ISessao, Sessao>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(10); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{

    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

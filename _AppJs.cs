namespace wwwcore;

class _AppJs {
    internal void Create() {
        var url = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "app.js");
        
        if(!File.Exists(url)) {
            string jsCode = @"
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('/serviceworker.js')
            .then(registration => {
                console.log('Service worker registered', registration);
            })
            .catch(error => {
                console.error('Service worker registration failed:', error);
            });
    }";
            
            File.WriteAllText(url, jsCode);
        }
    }
}


function makeAppConfig() {
  const date = new Date();
  const year = date.getFullYear();

  const AppConfig = {
    brand: 'CryptoBot',
    user: 'Tony',
    year,
    layoutBoxed: false,               // true, false
    navCollapsed: true,              // true, false
    navBehind: true,                 // true, false
    fixedHeader: true,                // true, false
    sidebarWidth: 'small',           // small, middle, large
    theme: 'dark',                   // light, gray, dark
    colorOption: '34',                // 11,12,13,14,15,16; 21,22,23,24,25,26; 31,32,33,34,35,36
    AutoCloseMobileNav: true,         // true, false. Automatically close sidenav on route change (Mobile only)
    productLink: '',
    apiUrl: 'http://localhost:63300/api/v1/'
  };

  return AppConfig;
}

export const APPCONFIG = makeAppConfig();

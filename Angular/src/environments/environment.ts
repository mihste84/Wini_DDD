export const environment = {
  production: false,
  title: 'Wini dev',
  env: 'local',
  msalConfig: {
    auth: {
      clientId: '8fe32187-5f45-4277-a5f5-21033c8d5d20',
      authority: 'https://login.microsoftonline.com/common',
      tenant: 'bb58bc62-9d26-41b7-b069-3d406229988b',
    },
  },
  graphConfig: {
    scopes: ['user.read'],
    uri: 'https://graph.microsoft.com/v1.0/me',
  },
  apiConfig: {
    scopes: ['api://cf4ed072-f4df-4552-8b42-cdbcc9a2802d/Bookings.Write', 'api://cf4ed072-f4df-4552-8b42-cdbcc9a2802d/Bookings.Read'],
    uri: 'https://localhost:7206/api/',
  },
};

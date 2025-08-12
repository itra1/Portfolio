const loopback = require('loopback');
const boot = require('loopback-boot');
const appManager = require('./app/appManager');
const PassportConfigurator = require('loopback-component-passport').PassportConfigurator;
const https = require('https');
const sslConfig = require('./ssl-config');

const app = module.exports = loopback();

let passportConfigurator = new PassportConfigurator(app);

app.on('uncaughtException', function (req, res, route, err) {
  try {

    app.models.DebugMailer.SendAdminMail("Server error: Ошибка обработки пакета", route + ' ' + err);

  } catch (e) { }
  if (!res.headersSent) {
    return res.send(500, { ok: false });
  }
  res.write('\n');
  res.end();
});

app.startSsl = function () {

  var options = {
    key: sslConfig.privateKey,
    cert: sslConfig.certificate,
  };

  server = https.createServer(options, app);

  // start the web server
  server.listen(3005, function () {
    app.emit('started');
    var baseUrl = 'https://' + app.get('host') + ':' + 3005;
    app.emit('started', baseUrl);
    //let baseUrl = app.get('url').replace(/\/$/, '');
    //var baseUrl = (httpOnly ? 'http://' : 'https://') + app.get('host') + ':' + app.get('port');
    console.log('Web server listening at: %s', baseUrl);
    if (app.get('loopback-component-explorer')) {
      let explorerPath = app.get('loopback-component-explorer').mountPath;
      console.log('Browse your REST API at %s%s', baseUrl, explorerPath);
    }

  });

  return server;
};

app.start = function () {

  return app.listen(app.get('port'), function () {
    var baseUrl = 'http://'+ app.get('host') + ':' + app.get('port');
    app.emit('started', baseUrl);
    console.log('LoopBack server listening @ %s%s', baseUrl, '/');
    if (app.get('loopback-component-explorer')) {
      var explorerPath = app.get('loopback-component-explorer').mountPath;
      console.log('Browse your REST API at %s%s', baseUrl, explorerPath);
    }
  });
};

boot(app, __dirname, function (err) {
  if (err) throw err;

  // start the server if `$ node server.js`
  if (require.main === module){
    app.start();
    app.startSsl();
  }
});

// Load the provider configurations
let config = {};
try {
  config = require('./providers.json');
} catch (err) {
  console.error('Please configure your passport strategy in `providers.json`.');
  console.error('Copy `providers.json.template` to `providers.json` and replace the clientID/clientSecret values with your own.');
  process.exit(1);
}
// Initialize passport
passportConfigurator.init();

// Set up related models
passportConfigurator.setupModels({
  userModel: app.models.user,
  userIdentityModel: app.models.userIdentity,
  userCredentialModel: app.models.userCredential
});
// Configure passport strategies for third party auth providers
for (let s in config) {
  let c = config[s];
  c.session = c.session !== false;
  passportConfigurator.configureProvider(s, c);
}

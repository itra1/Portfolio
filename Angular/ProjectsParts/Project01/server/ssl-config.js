var path = require('path');
var fs = require('fs');

exports.privateKey = fs.readFileSync(path.join(__dirname, './private/privateKeyRsa.pem')).toString();
exports.certificate = fs.readFileSync(path.join(__dirname, './private/certificate.pem')).toString();
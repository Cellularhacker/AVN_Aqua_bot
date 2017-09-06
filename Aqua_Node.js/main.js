/*
* 2017-09-06 10:43 KST
* Cellularhacker
* Project:AVN_Aqua_bot/main.js
* 
* This Application is based on "TG-CLI" (which made by Lua Script).
* So You need to install tg-cli and packages that it requires.
* 
***** <Linux Packages List> *****
* libreadline-dev libconfig-dev libssl-dev lua5.2 liblua5.2-dev libevent-dev libjansson-dev libpython-dev make
* 
***** <Getting a tg-cli> *****
* git clone --recursive https://github.com/vysheng/tg.git &amp;&amp; cd tg
* 
*/


const TelegramAPI = require('tg-cli-node');
const config = require('./config');

const Client = new TelegramAPI(config);

var ownId = 33242449;
var debug = true;
var msgdebug = false;

Client.connect(connection => {
    connection.on('message', message => {
        console.log('message:', message);
    });

    connection.on('error', e => {
        console.log('Error from Telegram API:', e);
    });

    connection.on('disconnect', () => {
        console.log('Disconnected from Telegram API');
    });
});

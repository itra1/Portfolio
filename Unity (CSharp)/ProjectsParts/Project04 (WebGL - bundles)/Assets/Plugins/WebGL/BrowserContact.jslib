var BrowserContact = {

    Subscribe: function() {
        uniti_init(function(dataSend) {
            SendMessage('BrowserContact', 'Callback', dataSend);
        });
		
		//SubscibeCookie(function(dataSend) {
        //    SendMessage('BrowserContact', 'SetBlockCookie', dataSend);
        //});
    },

    BattleRoundChange: function(roundId) {
        battle_round_change(Pointer_stringify(roundId));
    },

    BattleSelectPlayer: function(enemyId) {
        battle_select_player(Pointer_stringify(enemyId));
    },

    BattleEnd: function() {
        battle_end();
    },
    
    ShowBattleLog: function(battleId){
       open_battle_log(Pointer_stringify(battleId)); 
    },

    BattleSetAttack: function(data) {
        battle_set_attack(Pointer_stringify(data));
    },

    Auth: function(data) {
        get_auth(function(dataSend) {
            SendMessage('BrowserContact', 'SetAuth', dataSend);
        });
    },

	GetMapNum: function() {
        get_map_num(function(dataSend) {
            SendMessage('BrowserContact', 'SetMap', dataSend);
        });
	},
    
    OpenMercenaryList: function(){
        open_mercenary_list();
    },
    
    GetCookie: function(cookeiElement) {
        var dataCookie = battle_get_cookie(Pointer_stringify(cookeiElement));
        SendMessage('BrowserContact', 'SetCookie', dataCookie);
    },

    NetRequest: function(requestId, type, url, body, isAuth) {
        net_request(requestId, Pointer_stringify(type), Pointer_stringify(url), Pointer_stringify(body), function(code, response) {

		var netAnswer = {
            id: requestId,
            code: code,
            text: response
        };

            SendMessage('BrowserContact', 'NetCallback', JSON.stringify(netAnswer));
        }, isAuth);
    }

};

mergeInto(LibraryManager.library, BrowserContact);
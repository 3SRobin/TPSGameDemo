
import json
from common import conf
from dispatcher import Service
from common.events import ClientMsg
from Setting import KeyType


class LoginModule(Service):

    def __init__(self, server=None, sid=0):
        super(LoginModule, self).__init__(sid)
        self.server = server
        self.format = ClientMsg()
        self.register(conf.MODULE_LOGIN_FUN_LOGIN, self.playerLogin)
        self.register(conf.MODULE_LOGIN_FUN_REGISTER, self.playerRegister)

    def playerLogin(self, msg, client_id):
        loginJson = self.format.unmarshal(msg.data)
        loginMsg = json.loads(loginJson.msg_data)
        msgName = loginMsg[KeyType.NAME]
        msgPassword = loginMsg[KeyType.PASSWORD]
        password = self.server.DB.getPlayerPassword(msgName)
        if password is None or password != msgPassword:
            cmd = conf.LOGIN_WRONG
        else:
            cmd = conf.LOGIN_SUCCESSFUL
            print "login  ", msgName, msgPassword
        self.addMsgToServer(client_id, {"code": cmd, })

    def playerRegister(self, msg, client_id):
        registerJson = self.format.unmarshal(msg.data)
        registerMsg = json.loads(registerJson.msg_data)
        msgName = registerMsg[KeyType.NAME]
        msgPassword = registerMsg[KeyType.PASSWORD]
        data = self.server.DB.getPlayerData(msgName)
        if not data:
            self.server.DB.creatPlayer(name=msgName, score=0, blood=100, bullet=40, password=msgPassword)
            cmd = conf.REGISTER_SUCCESSFUL
            print "register  ", msgName, msgPassword
        else:
            cmd = conf.REGISTER_WRONG
        self.addMsgToServer(client_id, {"code": cmd, })

    def addMsgToServer(self, client_id, data):
        msg = self.server.getDataStream(conf.LOGIN_FEEDBACK, data)
        self.server.addCommonMsgToMQ(client_id, msg)

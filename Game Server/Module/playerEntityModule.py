import json
from common import conf
from dispatcher import Service
from common.events import ClientMsg
from Entity.playerEntity import PlayerEntity
from Setting import KeyType


class PlayerEntityModule(Service):

    def __init__(self, server=None, sid=0):
        super(PlayerEntityModule, self).__init__(sid)
        self.server = server
        self.format = ClientMsg()
        self.register(conf.MODULE_PLAYER_FUN_REGISTER, self.playerEntityRegister)
        self.register(conf.MODULE_PLAYER_FUN_DATA, self.playerEntityData)
        self.register(conf.MODULE_PLAYER_FUN_SAVE, self.playerEntityDataSave)

    def playerEntityRegister(self, msg, client_id):
        jsonStr = self.format.unmarshal(msg.data)
        msg = json.loads(jsonStr.msg_data)
        msgName = msg[KeyType.NAME]
        playerEntity = PlayerEntity()
        playerEntity.client_id = client_id
        playerEntity.name = msgName
        playerData = self.server.DB.getPlayerData(msgName)
        playerEntity.setPlayerData(playerData)
        self.server.registerEntity(KeyType.ONLINE_PLAYER, playerEntity)
        self.joinOtherPlayer(playerEntity)
        self.loadZombie(playerEntity)
        self.addMsgToServer(client_id, conf.PLAYER_REGISTER, playerData)

    def playerEntityData(self, msg, client_id):
        jsonStr = self.format.unmarshal(msg.data)
        msg = json.loads(jsonStr.msg_data)
        playerEntity = self.server.getPlayerByClientID(client_id)
        playerEntity.setPlayerData(msg)
        playerEntity.setPosition(msg[KeyType.POSITION])
        playerEntity.setRotation(msg[KeyType.ROTATION])
        msg[KeyType.ENTITY_ID] = playerEntity.entity_id
        self.addBroadcastMsgToServer(playerEntity, conf.OTHER_PLAYER_ACTION, msg)

    def playerEntityDataSave(self, msg, client_id):
        playerEntity = self.server.getPlayerByClientID(client_id)
        name = playerEntity.name
        bullet = playerEntity.bullet
        blood = playerEntity.blood
        score = playerEntity.score
        self.server.DB.updatePlayerData(name, bullet=bullet, score=score, blood=blood)
        data = {KeyType.ENTITY_ID : playerEntity.entity_id}
        self.addBroadcastMsgToServer(playerEntity, conf.OTHER_PLAYER_REMOVE, data)

    def joinOtherPlayer(self, playerEntity):
        for key, item in self.server.entities[KeyType.ONLINE_PLAYER].items():
            data = {}
            otherPlayer = {}
            if key != playerEntity.entity_id:
                otherPlayer[KeyType.ENTITY_ID] = item.entity_id
                otherPlayer[KeyType.POSITION] = [item.position.x, item.position.y + 1, item.position.z]
                otherPlayer[KeyType.ROTATION] = [item.rotation.x, item.rotation.z, item.rotation.z]
                self.addMsgToServer(playerEntity.client_id, conf.OTHER_PLAYER_JOIN, otherPlayer)
                data[KeyType.ENTITY_ID] = playerEntity.entity_id
                data[KeyType.POSITION] = [59, 4, -20]
                data[KeyType.ROTATION] = [playerEntity.rotation.x, playerEntity.rotation.z, playerEntity.rotation.z]
                self.addMsgToServer(item.client_id, conf.OTHER_PLAYER_JOIN, data)

    def loadZombie(self, playerEntity):
        for id, entity in self.server.entities[KeyType.ENEMY].items():
            msg = {
                    "entity_id": entity.entity_id,
                    "targetPosition": [entity.targetPosition.x, entity.targetPosition.y, entity.targetPosition.z],
                    "position": [entity.position.x, entity.position.y, entity.position.z]
                }
            self.addMsgToServer(playerEntity.client_id, conf.ZOMBIE_PRODUCE, msg)

    def addMsgToServer(self, client_id, msg_code, data):
        msg = self.server.getDataStream(msg_code, data)
        self.server.addCommonMsgToMQ(client_id, msg)

    def addBroadcastMsgToServer(self, playerEntity, msg_code, data):
        msg = self.server.getDataStream(msg_code, data)
        self.server.addBroadcastMsgToMQ(playerEntity.entity_id, msg)
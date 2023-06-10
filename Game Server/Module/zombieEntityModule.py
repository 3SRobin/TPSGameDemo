import json
from common import conf
from dispatcher import Service
from common.events import ClientMsg
from Entity.zombieEntity import ZombieEntity
from common_server.timer import TimerManager
from Setting import KeyType


class ZombieEntityModule(Service):

    def __init__(self, server=None, sid=0):
        super(ZombieEntityModule, self).__init__(sid)
        self.server = server
        self.format = ClientMsg()
        self.register(conf.MODULE_ZOMBIE_FUN_DATA, self.setZombieData)
        self.register(conf.MODULE_ZOMBIE_FUN_DEATH, self.zombieDeath)
        TimerManager.addRepeatTimer(conf.ZOMBIE_PRODUCE_TICK, self.zombieProduce)

    def zombieProduce(self):
        if self.server.entities[KeyType.ONLINE_PLAYER] is None or len(self.server.entities[KeyType.ENEMY]) >= conf.ZOMBIE_COUNT:
            return
        zombieEntity = ZombieEntity()
        self.server.registerEntity(KeyType.ENEMY, zombieEntity)
        for key, item in self.server.entities[KeyType.ONLINE_PLAYER].items():
            zombieEntity.setTargetPosition([item.position.x, item.position.y, item.position.z])
            break
        msg = {
            "entity_id": zombieEntity.entity_id,
            "targetPosition": [zombieEntity.targetPosition.x, zombieEntity.targetPosition.y, zombieEntity.targetPosition.z],
            "position": [2.0, 1.8, 13.0]
        }
        self.addBroadcastMsgToServer(None, conf.ZOMBIE_PRODUCE, msg)

    def setZombieData(self, msg, client_id):
        if self.server.entities[KeyType.ONLINE_PLAYER] is None:
            return
        jsonStr = self.format.unmarshal(msg.data)
        msg = json.loads(jsonStr.msg_data)
        if msg[KeyType.ENTITY_ID] in self.server.entities[KeyType.ENEMY].keys():
            zombieEntity = self.server.entities[KeyType.ENEMY][msg[KeyType.ENTITY_ID]]
            zombieEntity.blood = msg[KeyType.BLOOD]
            zombieEntity.setPosition(msg[KeyType.POSITION])
            for k, v in self.server.entities[KeyType.ONLINE_PLAYER].iteritems():
                if v.blood > 0:
                    zombieEntity.setTargetPosition([v.position.x, v.position.y, v.position.z])
                    break
            msg["targetPosition"] = [zombieEntity.targetPosition.x, zombieEntity.targetPosition.y, zombieEntity.targetPosition.z]
            self.addBroadcastMsgToServer(None, conf.ZOMBIE_DATA, msg)

    def zombieDeath(self,  msg, client_id):
        jsonStr = self.format.unmarshal(msg.data)
        msg = json.loads(jsonStr.msg_data)
        self.server.entities[KeyType.ENEMY].pop(msg[KeyType.ENTITY_ID])
        playerEntity = self.server.getPlayerByClientID(client_id)
        msg[KeyType.BLOOD] = 0
        self.addBroadcastMsgToServer(playerEntity, conf.ZOMBIE_DEATH, msg)

    def addBroadcastMsgToServer(self, playerEntity, msg_code, data):
        msg = self.server.getDataStream(msg_code, data)
        if playerEntity is None:
            self.server.addBroadcastMsgToMQ(-1, msg)
        else:
            self.server.addBroadcastMsgToMQ(playerEntity.entity_id, msg)
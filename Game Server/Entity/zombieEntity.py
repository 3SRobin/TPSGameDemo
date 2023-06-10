from Setting import KeyType
from Entity.entity import Entity,Position


class ZombieEntity(Entity):

    def __init__(self):
        super(ZombieEntity, self).__init__()
        self.blood = 100
        self.targetPosition = Position()

    def setPlayerData(self, data):
        self.blood = data[KeyType.BLOOD]

    def setTargetPosition(self, data):
        self.targetPosition.x = data[0]
        self.targetPosition.y = data[1]
        self.targetPosition.z = data[2]

    def tick(self):
        pass
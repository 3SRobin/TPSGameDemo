
from Setting import KeyType
from Entity.entity import Entity


class PlayerEntity(Entity):

    def __init__(self):
        super(PlayerEntity, self).__init__()
        self.client_id = -1

        self.name = ""
        self.blood = 0
        self.bullet = 0
        self.score = 0

    def setPlayerData(self, data):
        self.blood = data[KeyType.BLOOD]
        self.bullet = data[KeyType.BULLET]
        self.score = data[KeyType.SCORE]

    def tick(self):
        pass

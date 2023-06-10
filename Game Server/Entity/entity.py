

class Entity(object):

    def __init__(self):
        self.position = Position()
        self.rotation = Rotation()
        self.entity_id = -1

    def setPosition(self, pos):
        self.position.x = pos[0]
        self.position.y = pos[1]
        self.position.z = pos[2]

    def setRotation(self, rot):
        self.rotation.x = rot[0]
        self.rotation.y = rot[1]
        self.rotation.z = rot[2]

    def tick(self):
        return NotImplementedError


class Position(object):

    def __init__(self):
        self.x = 0
        self.y = 0
        self.z = 0


class Rotation(object):

    def __init__(self):
        self.x = 0
        self.y = 0
        self.z = 0
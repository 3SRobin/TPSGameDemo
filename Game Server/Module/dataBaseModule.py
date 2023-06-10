import sqlite3
from Setting import KeyType
from Module.dispatcher import Service


class DataBaseModule(Service):

    def __init__(self, server=None, sid=0):
        super(DataBaseModule, self).__init__(sid)
        self.path = "./Game.db"
        self.db_connect = sqlite3.connect(self.path)
        self.columns = ["name", "score", "blood", "bullet", 'password']
        self.server = server
        self.server.DB = self

    def createTable(self):
        cursor = self.db_connect.cursor()
        # create table
        try:
            cursor.execute('drop table PlayerTable;')
        except sqlite3.OperationalError as e:
            print e
        cursor.execute("create table PlayerTable("
                       "name char(50) primary key not null,"
                       "score int,"
                       "blood int,"
                       "bullet int,"
                       "password char(50) not null);")
        self.db_connect.commit()

    def initPlayerTable(self):
        self.createTable()
        # default Player
        db_cursor = self.db_connect.cursor()
        db_cursor.execute("insert into PlayerTable (name, score, blood, bullet, password)"
                          " values ('netease1', 0, 100, 40, 123);")
        db_cursor.execute("insert into PlayerTable (name, score, blood, bullet, password)"
                          " values ('netease2', 0, 100, 40, 123);")
        db_cursor.execute("insert into PlayerTable (name, score, blood, bullet, password)"
                          " values ('netease3', 0, 100, 40, 123);")
        self.db_connect.commit()

    def getPlayerData(self, name):
        cursor = self.db_connect.cursor()
        statement = "select * from PlayerTable where name = '" + name + "';"
        result = cursor.execute(statement).fetchone()
        if result:
            return {
                KeyType.NAME: result[0].encode('utf-8'),
                KeyType.SCORE: result[1],
                KeyType.BLOOD: result[2],
                KeyType.BULLET: result[3],
            }
        else:
            return None

    def getPlayerPassword(self, name):
        cursor = self.db_connect.cursor()
        statement = "select name,password from PlayerTable where name = '" + name + "';"
        result = cursor.execute(statement).fetchone()
        if result is None:
            return None
        else:
            return result[1].encode('utf-8')

    def creatPlayer(self, **kwargs):
        statement = "insert into PlayerTable ("
        valueStatement = "values ( "
        for k, v in kwargs.iteritems():
            if k in self.columns:
                statement += k + ', '
                if isinstance(v, str) or isinstance(v, unicode):
                    valueStatement += "'" + v + "', "
                elif isinstance(v, int) or isinstance(v, float):
                    valueStatement += str(v) + ", "
        statement = statement[:-2] + ") " + valueStatement[:-2] + ");"
        self.db_connect.cursor().execute(statement)
        self.db_connect.commit()
        return True

    def updatePlayerData(self, name, **kwargs):
        statement = 'update PlayerTable set '
        for k, v in kwargs.iteritems():
            if k in self.columns:
                statement += k + " = "
                if isinstance(v, str):
                    statement += "'" + v + "', "
                elif isinstance(v, int) or isinstance(v, float):
                    statement += str(v) + ", "
        statement = statement[:-2] + " where name = '" + name + "';"
        self.db_connect.cursor().execute(statement)
        self.db_connect.commit()
        return True

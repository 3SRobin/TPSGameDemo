# -*- coding: GBK -*-

import time
import json
import struct

import sys
sys.path.append('./common')

from Network.simpleHost import SimpleHost
from Module.dispatcher import Dispatcher
from common_server.timer import TimerManager
from common import conf
from common.events import ServerMsg
from Setting import KeyType
from Module.dataBaseModule import DataBaseModule
from Module.loginModule import LoginModule
from Module.playerEntityModule import PlayerEntityModule
from Module.zombieEntityModule import ZombieEntityModule


class Server(object):

	def __init__(self):
		super(Server, self).__init__()
		self.DB = None
		self._isRun = False
		self._entity_id = 0
		self.entities = {
			KeyType.ONLINE_PLAYER: {},
			KeyType.ENEMY: {},
		}
		self.MQ = {
			KeyType.COMMON: [],
			KeyType.BROADCAST: [],
		}
		self.host = SimpleHost()
		self.host.startup(addr=conf.SERVER_ADDR, port=conf.SERVER_PORT)
		self.dispatcher = Dispatcher()
		self.moduleRegister()

		# self.DB.initPlayerTable()                                     # run when the game.db was lose，create table
		# print self.DB.getPlayerData("netease3")
		# print self.DB.getPlayerPassword("netease1")

		return

	def moduleRegister(self):
		self.dispatcher.register(conf.MODULE_DATABASE, DataBaseModule(server=self, sid=conf.MODULE_DATABASE))
		self.dispatcher.register(conf.MODULE_LOGIN, LoginModule(server=self, sid=conf.MODULE_LOGIN))
		self.dispatcher.register(conf.MODULE_PLAYER, PlayerEntityModule(server=self, sid=conf.MODULE_PLAYER))
		self.dispatcher.register(conf.MODULE_ZOMBIE, ZombieEntityModule(server=self, sid=conf.MODULE_ZOMBIE))

	def generateEntityID(self):
		self._entity_id += 1
		return self._entity_id

	def getPlayerByClientID(self, client_id):
		for entity_id, entity in self.entities[KeyType.ONLINE_PLAYER].iteritems():
			if entity.client_id == client_id:
				return entity
		return None

	def registerEntity(self, entity_type, entity):
		eid = self.generateEntityID()
		entity.entity_id = eid
		self.entities[entity_type][eid] = entity
		return entity

	def tick(self):
		self.host.process()
		self.handleRecv()
		for eid, entity in self.entities.iteritems():                                   # 遍历游戏里的实体
			pass
			# Note: you can not delete entity in tick.
			# you may cache delete items and delete in next frame
			# or just use items.
			# entity.tick()
		self.handleMQ()                                                                 # 发送消息队列里的消息
		return

	def handleMQ(self):
		for client_id, msg in self.MQ[KeyType.COMMON]:
			self.host.sendClient(client_id, msg)

		for entity_id, msg in self.MQ[KeyType.BROADCAST]:
			if entity_id == -1:
				for id, entity in self.entities[KeyType.ONLINE_PLAYER].items():
					self.host.sendClient(entity.client_id, msg)
			else:
				for id, entity in self.entities[KeyType.ONLINE_PLAYER].items():
					if id != entity_id:
						self.host.sendClient(entity.client_id, msg)

		del self.MQ[KeyType.COMMON][:]
		del self.MQ[KeyType.BROADCAST][:]

	def handleRecv(self):
		flag, client_id, byteStream = self.host.read()
		while flag != conf.NET_CONNECTION_EMPTY:
			if flag == conf.NET_CONNECTION_NEW:
				code, client = self.host.getClient(client_id)
				print "new connect ", client.addr_port
				data = {"clientID": client_id, "state": "connect", }
				msg = self.getDataStream(conf.CONNECT, data)
				self.addCommonMsgToMQ(client_id, msg)
			elif flag == conf.NET_CONNECTION_LEAVE:
				player_entity = self.getPlayerByClientID(client_id)
				if player_entity is not None:
					msg = Msg(conf.MODULE_PLAYER, conf.MODULE_PLAYER_FUN_SAVE, "")
					self.dispatcher.dispatch(msg, client_id)
					self.entities[KeyType.ONLINE_PLAYER].pop(player_entity.entity_id)
			elif flag == conf.NET_CONNECTION_DATA:
				msg = Msg(*self.builtMsg(byteStream))
				self.dispatcher.dispatch(msg, client_id)
			flag, client_id, byteStream = self.host.read()

	def addCommonMsgToMQ(self, client_id, msg):
		self.MQ[KeyType.COMMON].append((client_id, msg))

	def addBroadcastMsgToMQ(self, enetity_id, msg):
		self.MQ[KeyType.BROADCAST].append((enetity_id, msg))

	def startServer(self):
		print "Game Server Start"
		if not self._isRun:
			self._isRun = True
			TimerManager.addRepeatTimer(conf.SERVER_TICK, self.tick)
		while self._isRun:
			time.sleep(0.001)
			TimerManager.scheduler()

	def getDataStream(self, msg_type, data):
		s = json.dumps(data)
		server_msg = ServerMsg(msg_type=msg_type, msg_data=s)
		return server_msg.marshal()

	def builtMsg(self, data):
		if len(data) > conf.NET_HEAD_LENGTH_SIZE:
			mid, fid = struct.unpack('=hh', data[:4])
			return mid, fid, data[4:]
		else:
			return -1, -1, data


class Msg(object):

	def __init__(self, mid, fid, data):
		self.mid = mid
		self.fid = fid
		self.data = data


def main():
	server = Server()
	server.startServer()


if __name__ == '__main__':
	main()




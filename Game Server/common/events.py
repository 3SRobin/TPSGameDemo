# -*- coding: GBK -*-
import conf
from header import SimpleHeader


class ServerMsg(SimpleHeader):
	def __init__(self, msg_type=0, msg_data=''):
		super(ServerMsg, self).__init__(conf.MSG_SERVER)
		self.appendParam('msg_type', msg_type, 'h')
		if len(msg_data) > 0:
			self.appendParam('msg_data', msg_data, 's')


class ClientMsg(SimpleHeader):
	def __init__(self, msg_data=''):
		super(ClientMsg, self).__init__(conf.MSG_CLIENT)
		self.appendParam('msg_data', msg_data, 's')
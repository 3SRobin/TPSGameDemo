# -*- coding: GBK -*-

import struct


class Header(object):
	BYTES_ORDER = '='

	def __init__(self, htype):
		super(Header, self).__init__()

		self.htype = htype
		self.hfmt = self.BYTES_ORDER + 'H'
		
		# bfmt to be defined in subclass and be updated when recieve new data
		self.bfmt = None
		self.raw = ''

		self.char_for_len = 'I'
		self.offset = struct.calcsize(self.BYTES_ORDER + self.char_for_len)
	
	def getFormat(self, raw):
		x = self.bfmt.count('%')
		if x == 0:                                                                  # x == 0 代表没有字符串
			return self.bfmt
		begin, elen, lst, fmt = 0, 0, [], self.bfmt
		self.offset = struct.calcsize(self.BYTES_ORDER + self.char_for_len)         # 记录字符串长度的变量的字节数  int型
		for i in xrange(x):
			end = fmt.index('%', begin)
			elen = elen + struct.calcsize(self.BYTES_ORDER + fmt[begin:end])
			s = struct.unpack(self.BYTES_ORDER + self.char_for_len, raw[elen - self.offset:elen])[0]
			elen = elen + s                                                         # 记录下一个字符串长度的字节开始位置
			lst.append(s)
			begin = end + len('%ds')
		if elen != 0:
			return fmt % tuple(lst)

	def marshal(self):
		self.raw = struct.pack(self.hfmt, self.htype)                               # 数据包类型
		ofmt, self.bfmt = self.bfmt, self.BYTES_ORDER + self.bfmt 
		self.raw = self.raw + self.imarshal()
		self.bfmt = ofmt
		return self.raw

	def unmarshal(self, raw=None):
		if raw is not None:
			self.raw = raw
		i = struct.calcsize(self.hfmt)                                                  # 数据包类型的字节数  short型的字节数
		# need not to unpack self.ytype, whitch is determined by class
		record = struct.unpack(self.hfmt, self.raw[0:i])
		if self.htype != record[0]:                                                     # 数据包的类型 检测
			raise TypeError('type dismatch when unmarshal.expect:%d,actual:%d' % (self.htype, record[0]))
		bfmt = self.BYTES_ORDER + self.getFormat(self.raw[i:])                          # 数据部分的格式
		record = struct.unpack(bfmt, self.raw[i:])
		self.iunmarshal(record)
		return self

	def imarshal(self):
		# pack attrs
		raise NotImplementedError

	def iunmarshal(self, data):
		# unpack attrs
		raise NotImplementedError


class SimpleHeader(Header):
	def __init__(self, msgtype):
		super(SimpleHeader, self).__init__(msgtype)

		self.bfmt = ''
		self.params_name = []
	
	def appendParam(self, pname, pvalue, ptype):
		# string param should be stored in length+data
		# so we append None pname
		if ptype.strip() == 's':                                                          # ‘s'代表字符串
			self.bfmt += self.char_for_len
			self.params_name.append(None)
			ptype = '%ds'

		self.bfmt += ptype
		self.params_name.append(pname)
		self.__setattr__(pname, pvalue)                                                   # 设置属性及其值

	def imarshal(self):
		values = []
		param_format = []

		last_param = True
		for pname in self.params_name:
			if pname:
				v = self.__getattribute__(pname)                                          # 获取属性的值
				if not last_param:                                                        # 为None 表示上一个为字符串的长度值
					values.append(len(v))
					param_format.append(len(v))
				values.append(v)
			last_param = pname

		return struct.pack(self.bfmt % tuple(param_format), *values)

	def iunmarshal(self, record):
		for i in range(len(record)):
			pname = self.params_name[i]
			if pname:
				self.__setattr__(pname, record[i])                                         # 设置属性


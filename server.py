# -*- coding: utf-8 -*-
import socket
import threading
import json
#解决通信问题

# 当新的客户端连入时会调用这个方法
def on_new_connection(client_executor, addr):
    print('Accept new connection from %s:%s...' % addr)
    while True:
      msg = client_executor.recv(1024).decode('utf-8')
      if True:
        #用户点击下载更新按钮
        if msg=='下载':
          with open('List.json','r',encoding='utf-8') as fr:
            sendData=json.load(fr)
            print("将要发送给客户端的信息为"+sendData)
            #通过python发送json信息，首先需要使用json.dumps将dict格式转化为str格式，但socket仅支持发送byte序列，因而还需要将str序列化，也就是repr()函数，再一个编码完成即可
            client_executor.send(bytes(repr(json.dumps(sendData)).encode('utf-8')))
          break
        #如果用户不是下载更新，就是上传数据
        elif msg=="exit":
          break
        elif msg=="上传1":
          msg = msg.strip()
          if msg!=0:
            #j=json.loads(msg)#json.loads()将str变成dict
            print("从客户端接收的信息为"+msg)
            with open('List.json','w') as fw1:
              json.dump(msg,fw1)
          break
        elif msg=="上传2": 
          msg = msg.strip()
          if msg!=0:
            print("从客户端接收的信息为"+msg)
            with open('ClockTime.json','w') as fw2:
              json.dump(msg,fw2)
          break
    client_executor.close()
    print('Connection from %s:%s closed.' % addr)

# 构建Socket实例、设置端口号和监听队列大小
listener = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
listener.bind(('0.0.0.0', 2333))
listener.listen(3)
print('Waiting for connect...')


#进入死循环，等待新的客户端连入。一旦有客户端连入，就分配一个线程去做专门处理。然后自己继续等待。
while True:
    client_executor, addr = listener.accept()
    t = threading.Thread(target=on_new_connection, args=(client_executor, addr))
    t.start()
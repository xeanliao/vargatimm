sudo nohup phantomjs --config=/opt/mapproxy/config.json /opt/mapproxy/mapImageServer.js -host 127.0.0.1 -port 9001 > /var/log/phantomjs.log 2>&1 &

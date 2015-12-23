# Accursed

Statistics tracker for Curse written using [Lapis](http://leafo.net/lapis/).

# Setup
Install [OpenResty](http://openresty.org/):
```bash
wget https://openresty.org/download/ngx_openresty-VERSION.tar.gz
tar xzvf ngx_openresty-VERSION.tar.gz
cd ngx_openresty-VERSION/
./configure # --with-pcre-jit --with-ipv6
make
sudo make install
```

Install [LuaRocks](https://luarocks.org/):
```bash
wget http://keplerproject.github.io/luarocks/releases/luarocks-VERSION.tar.gz
tar -xzvf luarocks-VERSION.tar.gz
cd luarocks-VERSION/
./configure --prefix=/usr/local/openresty/luajit \
    --with-lua=/usr/local/openresty/luajit/ \
    --lua-suffix=jit-2.1.0-alpha \
    --with-lua-include=/usr/local/openresty/luajit/include/luajit-2.1
make
sudo make install
```

Setup your path:
```bash
PATH=$PATH:/usr/local/openresty/luajit/bin
export PATH
```

Install the required packages:
```bash
# Run if you want to fail on the first error
# xargs -t -n 1 sh -c 'sudo luarocks install $0 || exit 255' < BoxFile
xargs -t -n 1 sh -c sudo luarocks install < BoxFile
```
Check that all commands ran successfully: MySQL may need to be configured manually.


[Install Node](https://nodejs.org/en/download/):
```bash
curl -sL https://deb.nodesource.com/setup_4.x | sudo -E bash -
sudo apt-get install -y nodejs
sudo npm install -g bower gulp
```

Build and serve.
```bash
moonc .
bower install
gulp

# Environment is either "development" or "production"
lapis migrate ENVIRONMENT && lapis build ENVIRONMENT
lapis server ENVIRONMENT
```
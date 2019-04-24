local cage = 
{
  -- import some packages:
  string = string,  --coroutine = coroutine,
  table = table, math = math,
  
  -- some 'global' functions:
  next = next, ipairs = ipairs, pairs = pairs,
  require = require, type = type,
  tonumber = tonumber, tostring = tostring,
  unpack = unpack, 
  setmetatable = setmetatable,
  getmetatable = getmetatable,

  -- modified global functions:
  --print = myprint,
  --error = myerror

  -- my own api:
  --move = move
  --kill = kill
}

local mt = {__index=cage}

function scriptLoader (scriptname, envName)
  local scriptenv = {}
  setmetatable (scriptenv, mt)
  
  local chunk  = loadfile (scriptname)
  setfenv (chunk, scriptenv)
  
  chunk()
  _G[envName] = scriptenv
end
---------------------------
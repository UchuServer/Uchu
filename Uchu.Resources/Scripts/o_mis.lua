require('Vector')

function emote(self,target, skillType)
       
        self:SetVar("EmbeddedTime", self:GetAnimationTime{  animationID = skillType }.time)
        self:PlayFXEffect{ priority = 1.5 ,effectType = skillType }
end

function showemote(self,target, skillID, skillType)
      --  self:ShowEffect {
      --  effectID     = skillID,
      --  type         = skillType,
    --}
end
Emote = { emote = emote}  

function getVarables(self)
    self:SetVar("myTarget", nil)
    self:SetVar("readyToAttack", true)
    self:SetVar("WPEvent_NUM", 1)   
    self:SetVar("AggroEmoteDelay", true) 
    self:SetVar("delayDone","Start")
    self:SetVar("aggrotarget",0)
    self:SetVar("delayDone","Start")
    self:SetVar("tetherOn",false)
    self:SetVar("Dead",false) 
    self:SetVar("myTarget",nil)
    self:SetVar("inpursuit",false)
    self:SetVar("AggroOnce", 0 ) 
    self:SetVar("HearBeat", false)  
end 
function loadOnce(self)

	self:SetVar("AttackingTarget", "NoTarget" )
    -- Lua Attachs Way Point Set
    if self:GetVar("Set.WayPointSet") ~= nil then
        self:SetVar("attached_path", self:GetVar("Set.WayPointSet")) 
    end
    
    -- Check if NPC is attached to a WP Set. 
    if self:GetVar("Set.SuspendLuaMovementAI") == nil or self:GetVar("Set.SuspendLuaMovementAI") == false then
		if self:GetVar("attached_path") ~= nil then
			self:FollowWaypoints()
		end 
    end
    self:SetVar("PointInLine",false)
    self:SetVar("FollowState", "start") 
    --self:SetVar("PointInLine",1)

    Child_Timer = {}
    local skill = self:GetSkills().skills[1];
    if (skill ~= 0) then
        self:SetVar("SkillTime", self:GetSkillInfo{skillID = skill}.cooldown)
    end
    
    self:SetVar("Child_Timer", Child_Timer )  
    self:SetVar("MaxPet", self:GetVar("Set.Pet_Count")) 
 
 
    hp = self:GetMaxHealth{}.health
    self:SetHealth{ health = hp }
    self:SetVar("inpursuit",false)
    self:SetVar("aggrotarget",0)  
    if self:GetVar("Set.OverRideName") then
        self:SetName { name = self:GetVar("Set.Name")  }
    end
    if self:GetVar("Set.OverRideHealth") then
         self:SetMaxHealth{ health = self:GetVar("Set.Health") }
         self:SetHealth{ health = self:GetVar("Set.Health") }
    end


end
--///////////////////////////////////////////////////////////////////
-- Delay
local function shift(fn)
    coroutine.yield(fn)
end
--///////////////////////////////////////////////////////////////////
-- Reset State 
function SetResetState(self) 
      getVarables(self)
      self:SetVar("inpursuit",false)
      self:SetVar("AttackingTarget", "NoTarget" )
      self:SetTetherPoint { tetherPt = self:GetPosition().pos,radius = 0 }
     
     if self:GetVar("Set.SuspendLuaMovementAI") == nil or self:GetVar("Set.SuspendLuaMovementAI") == false then
		 if  self:GetVar("attached_path") ~= nil then    
			   self:FollowWaypoints()
			  return
		 end

		 if self:GetVar("Set.MovementType") == "Wander" then
			setState("Meander", self)    
			return
		 end
     end
      
      if self:GetVar("Set.MovementType") == "Guard" then
         setState("Idle", self)    
         return
      end  
 

end


local function proc(fn)
    local coro = coroutine.wrap(fn)
    local cont = function (...)
        local cb = coro(unpack(arg))
        if cb then cb() end
    end
    cont(cont)
end

Delay = {
    proc = proc,
    shift = shift,
}
-- 
--------------------------------------------------------
-- Targeting / Storing 
--------------------------------------------------------
Var = {}
Var.__index = Var

function Var.__G(self,x)
    self:GetVar(x)
    return x
end
function Var.__S(self,x)
    self:SetVar(x)
end

function storeTarget(self, target)
    idString = target:GetID()
    finalID = "|" .. idString
    self:SetVar("myTarget", finalID)
end





function getMyTarget(self)
    targetID = self:GetVar("myTarget")
    return GAMEOBJ:GetObjectByID(targetID)
end
function storePet(self,ID, name , num )
    idString = ID:GetID()
    finalID = "|" .. idString
    self:SetVar(name..num , finalID)
end 

function getPet(self, name , num )
    targetID = self:GetVar(name..num)
    return GAMEOBJ:GetObjectByID(targetID)
end

function getIDany(self, string ,holder) 
    targetID = holder:GetVar(string)
    return GAMEOBJ:GetObjectByID(targetID)
end 

function getParent(self)
    targetID = self:GetVar("My_Parent_ID")
    return GAMEOBJ:GetObjectByID(targetID)
end

function getPetID(self,num)
    targetID = self:GetVar("StoredPet."..num)
    return GAMEOBJ:GetObjectByID(targetID)
end


function storeParent(self, target)
    idString = self:GetID()
    finalID = "|" .. idString
    target:SetVar("My_Parent_ID", finalID)
end

function storeHomePoint(self)
    mypos = self:GetPosition().pos 
    myX = self:SetVar("myx", mypos.x)
    myY = self:SetVar("myy", mypos.y)
    myZ = self:SetVar("myz", mypos.z)
    rot = self:GetRotation()
    self:SetVar("rot",rot) 

end

function getHomePoint(self)
    pos = {}
    pos.x = self:GetVar("myx")
    pos.y = self:GetVar("myy")
    pos.z = self:GetVar("myz") 
    return pos
end

function storeMeanderPoint(self)
    mypos = self:GetPosition().pos 
    myX = self:SetVar("Meadx", mypos.x)
    myY = self:SetVar("Meady", mypos.y)
    myZ = self:SetVar("Meadz", mypos.z)
end

function getMeanderPoint(self)
    pos = {}
    pos.x = self:GetVar("Meadx")
    pos.y = self:GetVar("Meady")
    pos.z = self:GetVar("Meadz") 
    return pos
end
--------------------------------------------------------------
-- returns turns a random  pos. radius = distance
--------------------------------------------------------------
function getRandomPos(self,myPos,radius)

        PoSSet = radius + radius
        PoSMin = radius - PoSSet
        PoSMax = radius
        PoS = {}
        PoS.x = myPos.x + math.random (PoSMin, PoSMax)
        PoS.z = myPos.z + math.random (PoSMin, PoSMax)
        PoS.y = myPos.y 
        return PoS

end


function getRandomDelay(self)
             Min = self:GetVar("Set.WanderDelayMin")
             Max = self:GetVar("Set.WanderDelayMax")
             ran = math.random(Min,Max)
    return ran
end
--------------------------------------------------------------
-- returns turns a random Flee pos. radius = dis to flee
--------------------------------------------------------------

function getRandomFleePos(self,myPos,radius)

        PoSSet = radius + radius
        PoSMin = radius - PoSSet
        PoSMax = radius
        PoS = {}
        PoS.x = myPos.x + math.random (PoSMin, PoSMax)
        PoS.z = myPos.z + radius
        PoS.y = myPos.y 
        return PoS
end

--------------------------------------------------------------
-- returns the Heading of an Object
--------------------------------------------------------------
function getHeading(obj)
    local q = obj:GetRotation()
        return({
                x = 2 * q.y * q.w + 2 * q.x * q.x,
                y = 2 * q.z * q.y - 2 * q.x * q.w,
                z = q.z*q.z + q.w*q.w - q.x*q.x - q.y*q.y,
            })

end
--[[
--------------------------------------------------------------
-- Splits a string.   ## 
Sample :::
    local string = "1,2,Alpah"
    loacl final = split(string,",") 
    final returns a table with
    [1] = 1
    [2] = 2
    [3] = Alpah
--------------------------------------------------------------
--]]
function split(str, pat)
   local t = {}  -- NOTE: use {n = 0} in Lua-5.0
   local fpat = "(.-)" .. pat
   local last_end = 1
   local s, e, cap = str:find(fpat, 1)
   while s do
      if s ~= 1 or cap ~= "" then
	 table.insert(t,cap)
      end
      last_end = e+1
      s, e, cap = str:find(fpat, last_end)
   end
   if last_end <= #str then
      cap = str:sub(last_end)
      table.insert(t, cap)
   end
   return t
end
--------------------------------------------------------------
-- returns the Distance between A and B
--------------------------------------------------------------
function dist(a ,b)
   local dx = math.abs(b.x - a.x)
   local dy = math.abs(b.y - a.y)
   local dz = math.abs(b.z - a.z)
   local d = math.sqrt((dx ^ 2) + (dy ^ 2) + (dz ^ 2))
   return d
end 

--------------------------------------------------------------
-- store an object by name
--------------------------------------------------------------
function storeObjectByName(self, varName, object)

    idString = object:GetID()
    finalID = "|" .. idString
    self:SetVar(varName, finalID)
   
end

--------------------------------------------------------------
-- get an object by name
--------------------------------------------------------------
function getObjectByName(self, varName)

    targetID = self:GetVar(varName)
    if (targetID) then
		return GAMEOBJ:GetObjectByID(targetID)
	else
		return nil
	end
	
end


--------------------------------------------------------------
-- register with zone control object
--------------------------------------------------------------
function registerWithZoneControlObject(self)

    -- register with zone control object
    GAMEOBJ:GetZoneControlID():ObjectLoaded{ objectID = self, templateID = self:GetLOT().objtemplate }
	
end

--------------------------------------------------------------
-- play actions on an object --------- CLIENT ONLY

-- Must ADD the Following to your Script
--CONSTANTS = {}
--CONSTANTS["NO_OBJECT"] = "0"
--------------------------------------------------------------
function DoObjectAction(actor, type, action)

    -- spatial chat
    if (type == "chat") then
	    actor:DisplayChatBubble{wsText = action}
    
    -- animation
    elseif (type == "anim") then
	    local anim_time = actor:GetAnimationTime{  animationID = action }.time
	    if (tonumber(anim_time) > 0) then
			actor:PlayAnimation{animationID = action}
		end
 
	-- effect
    elseif (type == "effect") then
		actor:PlayFXEffect{name = "N_" .. action, effectType = action }

	elseif (type == "stopeffects") then
	   	actor:StopFXEffect{ name = "N_" .. action }
	
    end

end


--------------------------------------------------------------
function round(num, idp)
  local mult = 10^(idp or 0)
  return math.floor(num * mult + 0.5) / mult
end
--------------------------------------------------------------
--------------------------------------------------------------
-- Table to String

function implode(d,p)
  if p ~= nil then
      local newstr
      newstr = ""
      if(#p == 1) then
        return p[1]
      end
      for ii = 1, (#p-1) do
        newstr = newstr .. p[ii] .. d
      end
      newstr = newstr .. p[#p]
      return newstr
  end
end
--------------------------------------------------------------
--------------------------------------------------------------
---// CHILL CODE ™ //--
-- table.ordered( [sorted reverse], [type] )  v 2

-- Lua 5.x add-on for the table library
-- Table using sorted index, with binary table for fast lookup.
-- http://lua-users.org/wiki/OrderedTable by PhilippeFremy 

-- table.ordered( [sorted reverse], [type] )
-- Gives you back a ordered table, can only take entered type
-- as index returned by type(index), by default "string"
-- sorted reverse, sorts the table in reverse order, else normal
-- stype is the deault index type returned by type( index ),
-- by default "string", it is only pssible to set one type as index
-- will effectively create a binary table, and will always lookup
-- through binary when an index is called
function table.ordered(ireverse, stype)
  local newmetatable = {}
  
  -- set sort function
  if ireverse then
    newmetatable._ireverse = 1
    function newmetatable.fcomp(a, b) return b[1] < a[1] end
  else
    function newmetatable.fcomp(a, b) return a[1] < b[1] end
  end

  -- set type by default "string"
  newmetatable.stype = stype or "string"

  -- fcomparevariable
  function newmetatable.fcompvar(value)
    return value[1]
  end

  -- sorted subtable
  newmetatable._tsorted = {}

  -- behaviour on new index
  function newmetatable.__newindex(t, key, value)
    if type(key) == getmetatable(t).stype then
      local fcomp = getmetatable(t).fcomp
      local fcompvar = getmetatable(t).fcompvar
      local tsorted = getmetatable(t)._tsorted
      local ireverse = getmetatable(t)._ireverse
      -- value is given so either update or insert newly
      if value then
        local pos, _ = table.bfind(tsorted, key, fcompvar, ireverse)
        -- if pos then update the index
        if pos then
          tsorted[pos] = {key, value}
        -- else insert new value
        else
          table.binsert(tsorted, {key, value}, fcomp)
        end
      -- value is nil so remove key
      else
        local pos, _ = table.bfind(tsorted, key, fcompvar, ireverse)
        if pos then
          table.remove(tsorted, pos)
        end
      end
    end
  end

  -- behavior on index
  function newmetatable.__index(t, key)
    if type(key) == getmetatable(t).stype then
      local fcomp = getmetatable(t).fcomp
      local fcompvar = getmetatable(t).fcompvar
      local tsorted = getmetatable(t)._tsorted
      local ireverse = getmetatable(t)._ireverse
      -- value if key exists
      local pos, value = table.bfind(tsorted, key, fcompvar, ireverse)
      if pos then
        return value[2]
      end
    end
  end

  -- set metatable
  return setmetatable({}, newmetatable)
end
		
--// table.binsert( table, value [, comp] )

-- Lua 5.x add-on for the table library
-- Binary inserts given value into the table sorted by [,fcomp]
-- fcomp is a comparison function that behaves just like
-- fcomp in table.sort( table [, comp] ).
-- This method is faster than doing a regular
-- table.insert(table, value) followed by a table.sort(table [, comp]).
function table.binsert(t, value, fcomp)
  -- Initialize compare function
  local fcomp = fcomp or function(a, b) return a < b end

  -- Initialize numbers
  local iStart, iEnd, iMid, iState =  1, table.getn( t ), 1, 0

  -- Get insert position
  while iStart <= iEnd do
    -- calculate middle
    iMid = math.floor((iStart + iEnd) / 2)

    -- compare
    if fcomp(value , t[iMid]) then
      iEnd = iMid - 1
      iState = 0
    else
      iStart = iMid + 1
      iState = 1
    end
  end

  local pos = iMid+iState
  table.insert(t, pos, value)
  return pos
end


--// table.bfind(table, value [, compvalue] [, reverse])

-- Lua 5.x add-on for the table library.
-- Binary searches the table for value.
-- If the value is found it returns the index and the value of
-- the table where it was found.
-- fcompval, if given, is a function that takes one value and
-- returns a second value2 to be compared with the input value,
-- e.g. compvalue = function(value) return value[1] end
-- If reverse is given then the search assumes that the table
-- is sorted with the biggest value on position 1.

function table.bfind(t, value, fcompval, reverse)
  -- Initialize functions
  fcompval = fcompval or function(value) return value end
  fcomp = function(a, b) return a < b end
  if reverse then
    fcomp = function(a, b) return a > b end
  end

  -- Initialize Numbers
  local iStart, iEnd, iMid = 1, table.getn(t), 1

  -- Binary Search
  while (iStart <= iEnd) do
    -- calculate middle
    iMid = math.floor((iStart + iEnd) / 2)

    -- get compare value
    local value2 = fcompval(t[iMid])

    if value == value2 then
      return iMid, t[iMid]
    end

    if fcomp(value , value2) then
      iEnd = iMid - 1
    else
      iStart = iMid + 1
    end
  end
end

-- Iterate in ordered form
-- returns 3 values i , index, value
-- ( i = numerical index, index = tableindex, value = t[index] )
function orderedPairs(t)
  return orderedNext, t
end
function orderedNext(t, i)
  i = i or 0
  i = i + 1
  local indexvalue = getmetatable(t)._tsorted[i]
  if indexvalue then
    return i, indexvalue[1], indexvalue[2]
  end
end
function SecondsToClock(sSeconds)
	local nSeconds = sSeconds
	
		if nSeconds == 0 then
		
		
			return "00:00";
			else
			nHours = string.format("%02.f", math.floor(nSeconds/3600));
			nMins = string.format("%02.f", math.floor(nSeconds/60 - (nHours*60)));
			nSecs = string.format("%02.f", math.floor(nSeconds - nHours*3600 - nMins *60));
			return nMins..":"..nSecs
		end
end



function SendNetWorkVar( actor , name , object1, object2, string1, string2, int1, int2 )

	local MSG = {}
	
	MSG["name"] = name
	MSG["Object1"] = object1
	MSG["Object2"] = object2
	MSG["String1"] = string1
	MSG["String2"] = string2
	MSG["int1"] = int1
	MSG["int2"] = int2
	
	actor:SetNetworkVar(MSG)
		
		
end

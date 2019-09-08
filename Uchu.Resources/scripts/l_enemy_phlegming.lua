require('State')
require('o_StateCreate')
require('o_Main')


CONSTANTS = {}
CONSTANTS["PATH1"] = "TestPath"
CONSTANTS["PATH2"] = "TestPath2"
CONSTANTS["path_jump"] = 16
CONSTANTS["path_jump_height"] = 148.1
CONSTANTS["path2_jump"] = 999
CONSTANTS["path2_win"] = 21
CONSTANTS["death_height"] = 133


function onStartup(self) 
	local pos = self:GetPosition().pos;
	self:SetVar("x", pos.x)
	self:SetVar("z", pos.z)
	self:SetVar("time", 0)
			GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "HeightCheck",self ) 

end

onTimerDone = function(self, msg)
	--print (msg.name)
 if (msg.name =="HeightCheck") then
	local pos = self:GetPosition().pos;
	local x = self:GetVar("x")
	local z = self:GetVar("z")
	local time = self:GetVar("time")
	
	pos.y = 0
  local myPos = Vector.new(pos)
  local lastPos = Vector.new(x,0,z)	
  
  local diff = myPos-lastPos
  local dist = diff:sqrLength()
  
  if (time > 2) then
  	if (dist < 16) then
			local curPath = self:GetVar("CurPath")
			if (curPath ~= nil) then
				print ("early reverse")
				self:SetCurrentPath{pathName=curPath, bReverse=true}  		
			end
  	end
  end
  
	local pos = self:GetPosition().pos;
	self:SetVar("x", pos.x)
	self:SetVar("z", pos.z)
	self:SetVar("time", time+1)  
	--print (pos.y)
	if (pos.y < CONSTANTS["death_height"]) then
				self:Die{killerID = self}
		else
				GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "HeightCheck",self ) 
		end
 end
end

function onArrived(self, msg)
	self:SetVar("CurPath", msg.pathName)
	
	-- get waypoint data
	--local curWP = self:GetVar("CurWaypoint")
		print(msg.pathName..":".. msg.wayPoint)

	if (msg.pathName == CONSTANTS["PATH1"]) then
	
		if (msg.wayPoint == CONSTANTS["path_jump"]) then
			local pos = self:GetPosition().pos
			if (pos.y > CONSTANTS["path_jump_height"]) then
				self:SetCurrentPath{pathName=CONSTANTS["PATH2"]}
	--			self:SetVar("CurWaypoint", 0)
			--else
--				self:SetVar("CurWaypoint", 1)
			end
		end
	end
	

	if (msg.pathName == CONSTANTS["PATH2"]) then
--		if (msg.wayPoint == CONSTANTS["path2_jump"]) then
--			if (curWP ~= nil and curWP > 0) then
--				self:SetCurrentPath{pathName=CONSTANTS["PATH1"]}
--				self:SetVar("CurWaypoint", 0)
--			else
--				self:SetVar("CurWaypoint", 1)
--			end
--		end
		if (msg.wayPoint == CONSTANTS["path2_win"]) then
			getParent(self):UpdateMissionTask{taskType = "phlegming_win", target=self}
			self:SetVar("win", 1)
		end
	end
	
	--getParent(self):Arrived{pathName = msg.pathName, wayPoint = msg.wayPoint}

end

function onDie(self, msg)
	if (self:GetVar("win") == nil) then
		getParent(self):UpdateMissionTask{taskType = "phlegming_die", target=self}
	end
end

function onPathStuck(self, msg)
	local curPath = self:GetVar("CurPath")
	self:SetCurrentPath{pathName=curPath, bReverse=true}
end

















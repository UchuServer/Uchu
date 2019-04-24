-----------------------------------------------------------
--spawns random bad guys from each maelstrom object destroyed
-----------------------------------------------------------



local maxToSpawn = 3
local chanceToSpawn = 4
local enemyType1 = 7815
local enemyType2 = 6789
local enemyType3 = 6806


--[[function onStartup(self)

   print("starting up")

end--]]


function onDie(self, msg)
   
   local pos = self:GetPosition().pos
   
   local config = { {"tetherRadius", 120 }, {"aggroRadius", 100}, {"wanderRadius", 70 }}
   
   local spawnThisType = math.random(chanceToSpawn)
   
   local spawnThisMany = math.random(maxToSpawn)
   
   --print("spawnThisMany = " .. spawnThisMany)
   
   if spawnThisType == 1 or spawnThisType == 2 then
      
      --print("spawning ronin")
      for i =1, spawnThisMany do
   
         RESMGR:LoadObject { objectTemplate = enemyType1  , x = pos.x , y =  pos.y , z = pos.z , owner = self,configData = config }
      
      end
   
   elseif spawnThisType == 3 then
   
      --print("spawning pirates")
      for i = 1, spawnThisMany do
         
         RESMGR:LoadObject { objectTemplate = enemyType2  , x = pos.x , y =  pos.y , z = pos.z , owner = self,configData = config }
      
      end
   
   elseif spawnThisType == 4 then
   
      --print("spawning ape")
      --for i = 1, spawnThisMany do
         
      RESMGR:LoadObject { objectTemplate = enemyType3  , x = pos.x , y =  pos.y , z = pos.z , owner = self,configData = config }
   end
   --[[RESMGR:LoadObject { objectTemplate = 7815  , x = pos.x +2, y =  pos.y , z = pos.z +2, owner = self,configData = config }
   RESMGR:LoadObject { objectTemplate = 7815  , x = pos.x -2, y =  pos.y , z = pos.z -2, owner = self,configData = config }--]]
   
end
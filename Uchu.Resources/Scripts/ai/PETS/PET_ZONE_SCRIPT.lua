--[[
require('o_mis')


-- stores all flowers
FLOWERS = {}



function onTimerDone(self, msg)

    	if ( msg.name == "start" ) then

         	--print("spawning Pet-Nodes") 
	      ReSpawnChild(self)

	end

	if ( msg.name == "die" ) then

      	DeadChild(self)
           	--print("Re----spawning Pet-Node") 

    	end   

end


function onStartup(self)
 
    self:SetVar("NumberOfChildren", 0 ) -- Dont not change this Val <<< 
    self:SetVar("WP_Num", 1)           -- Dont not change this Val <<<    
    --/////////////////////////////////////////////
    -- Settings  (Alpha =a) " a_1 (Num = 1) a_1 = the WayPoint Set
    --/////////////////////////////////////////////
    self:SetVar("PetNames", "nodes") 
    self:SetVar("WP_Alpha", "t")   -- starting letter of the Way Point ( a_1 = a)  or (anyName_1) 
    self:SetVar("MaxPets", 9)      -- Max Pets Spawn in the wrold
    self:SetVar("TotalPets", 9)    -- Total Number of Pets
    self:SetVar("TreasureID", 3495) 
    --/////////////////////////////////////////////
    --/////////////////////////////////////////////
    -- define total pet IDs
    for i = 1, self:GetVar("TotalPets") do
         self:SetVar("Pet_"..i, self:GetVar("TreasureID"))
         -- attach WP to NPC
         self:SetVar("slot_"..i, nil ) 
    end  
   
   
    for i = 1,  self:GetVar("TotalPets")  do  
        local FinalName = (self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..i)
        self:SetVar(FinalName, "NotSpawned") 
    end
     GAMEOBJ:GetTimer():AddTimerWithCancel(15, "start",self )
end



 
function DeadChild(self)

	-- Remove a child node from the number of nodes
      local NChildren =  self:GetVar("NumberOfChildren") - 1 
      self:SetVar("NumberOfChildren", NChildren ) 

	-- Spawn a new child in
      ReSpawnChild(self)

end 

function ReSpawnChild(self) 

        if  self:GetVar("NumberOfChildren") < self:GetVar("MaxPets") then

             local ran = GetValidRandom(self) 

             for i = 1,  self:GetVar("TotalPets")  do  

                	local FinalName = (self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..i)

                	if ran == i and self:GetVar(FinalName) == "NotSpawned" then
                    	SpawnPet(self, i ) 
               	end

             end 
       end
end

onChildLoaded = function(self,msg)
     
	if  msg.childID:GetLOT().objtemplate ==  self:GetVar("TreasureID") then
      	local FinalName = (self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..self:GetVar("ChildLoadNUM"))
            msg.childID:SetVar("SpawnedVar", FinalName )
            local FreeSlot = GetValidRandom(self) 
           
            msg.childID:SetVar("attached_path",  self:GetVar("WP_Alpha").."_"..self:GetVar("ChildLoadNUM"))
            msg.childID:SetVar("I_Have_A_Parent", true )
            storeParent(self,  msg.childID)
	end

end 


--************************************************************************************
--**   Functions 
--************************************************************************************
function SpawnPet(self, num ) 
     
	for i = 1,  self:GetVar("TotalPets")  do  
        
		if ( num == i ) then
			local firstWP = GAMEOBJ:GetWaypointPos( self:GetVar("WP_Alpha").."_"..i, 1)
            	self:SetVar("ChildLoadNUM", i ) 
             	--local config = { {"markedAsPhantom", true} }
             	RESMGR:LoadObject { objectTemplate =  self:GetVar("TreasureID"),  x=  firstWP.x  , y=  firstWP.y , z=  firstWP.z  , owner = self,  configData = config } -- A1  
             	self:SetVar(self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..i, "Spawned" )  
             	local NChildren =  self:GetVar("NumberOfChildren") + 1 
             	self:SetVar("NumberOfChildren", NChildren ) 
             	break
        	end 
        
     	end

	if  self:GetVar("NumberOfChildren") < self:GetVar("MaxPets") + 1 then
    
	  	ReSpawnChild(self) 

	end 

end

function GetValidRandom(self) 

    while true do 
      	NUM = math.random(1,self:GetVar("TotalPets"))  
	      FinalName = (self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..NUM)
        	
		if self:GetVar(FinalName) == "NotSpawned" then 
            	break
       	end
    end 

    return NUM 
end 


function onNotifyObject(self, msg)

	if (msg.name == "died") then
      
		GAMEOBJ:GetTimer():AddTimerWithCancel( 20, "die",self )

	end
end



--------------------------------------------------------------
-- When objects are loaded via zone notification
--------------------------------------------------------------
function onObjectLoaded(self, msg)
	if ( msg.templateID == 3646 ) then
		local nextFlower = #FLOWERS + 1
        FLOWERS[nextFlower] = msg.objectID:GetID()
	end
end



--------------------------------------------------------------
-- called when a player is loaded and ready
--------------------------------------------------------------
function onPlayerLoaded(self, msg)
	
	-- send the player's ID to the flowers so each of them can tell the newly-loaded client which anim to use
	for flowerID = 1, #FLOWERS do
        local flower = GAMEOBJ:GetObjectByID(FLOWERS[flowerID])
        flower:NotifyObject{ ObjIDSender = msg.playerID, name = "playerLoaded" }
	end

end
]]--
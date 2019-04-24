require('o_mis')
require('State')
require('o_StateCreate')
require('o_WayPoints')
--///////////////////////////////////////////////////////////////////////////////////////
--//            Seige Dir
--///////////////////////////////////////////////////////////////////////////////////////

function onStartup(self)
    self:SetVar("NumberOfChildern", 0 ) -- Dont not change this Val <<< 
    self:SetVar("WP_Num", 1)           -- Dont not change this Val <<<    
    --/////////////////////////////////////////////
    -- Settings  (Alpha =a) " a_1 (Num = 1) a_1 = the WayPoint Set
    --/////////////////////////////////////////////
    self:SetVar("PetNames", "nin") 
    self:SetVar("WP_Alpha", "b")   -- starting letter of the Way Point ( a_1 = a)  or (anyName_1) 
    self:SetVar("MaxPets", 3)      -- Max Pets Spawn in the wrold
    self:SetVar("TotalPets", 6)    -- Total Number of Pets
    self:SetVar("PetID", 2245) 
    --/////////////////////////////////////////////
    --/////////////////////////////////////////////
    -- define total pet IDs
    for i = 1, self:GetVar("TotalPets") do
         self:SetVar("Pet_"..i, self:GetVar("PetID"))
         -- attach WP to NPC
         self:SetVar("slot_"..i, nil ) 
    end  
   
   
    for i = 1,  self:GetVar("TotalPets")  do  
        local FinalName = (self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..i)
        self:SetVar(FinalName, "NotSpawned") 
    end
 
 
    self:UseStateMachine{} 

    ParentIdle = State.create()
    ParentIdle.onEnter = function(self)
         setState("ReSpawnChild",self)
    end 
    ParentIdle.onArrived = function(self)
         
    end    


    DeadChild = State.create()
    DeadChild.onEnter = function(self)
        local NChildern =  self:GetVar("NumberOfChildern") - 1 
        self:SetVar("NumberOfChildern", NChildern ) 
        setState("ReSpawnChild",self)
    end 
    DeadChild.onArrived = function(self)
         
    end 
    ----------------------------------------------------------------------------------
     -- ///////////////////////////////////////////////////////////////////////////////
    -- ReSpawn Childern State
    -- ///////////////////////////////////////////////////////////////////////////////
        ReSpawnChild = State.create()
        ReSpawnChild.onEnter = function(self) 

            if  self:GetVar("NumberOfChildern") < self:GetVar("MaxPets") then
                 local ran = GetValidRandom(self) 
                 for i = 1,  self:GetVar("TotalPets")  do  
                    local FinalName = (self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..i)
                    if ran == i and self:GetVar(FinalName) == "NotSpawned" then
                        SpawnPet(self, i ) 
                   end
               end 
           end
        end
     ReSpawnChild.onArrived = function(self)
         
    end        
    ----------------------------------------------------------------------------------   
    addState(ParentIdle, "ParentIdle", "ParentIdle", self)
    addState(DeadChild, "DeadChild", "DeadChild", self ) 
    addState(ReSpawnChild,"ReSpawnChild","ReSpawnChild",self) 
    beginStateMachine("ParentIdle", self) 
    ParentIdle.onEnter(self)
    
end
onChildLoaded = function(self,msg)
     
        
            local FinalName = (self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..self:GetVar("ChildLoadNUM"))
            msg.childID:SetVar("SpawnedVar", FinalName )
            local FreeSlot = GetValidRandom(self) 
           
            msg.childID:SetVar("attached_path",  self:GetVar("WP_Alpha").."_"..self:GetVar("ChildLoadNUM"))
            msg.childID:SetVar("I_Have_A_Parent", true )
            storeParent(self,  msg.childID)
         
       
    

end 
--************************************************************************************
--**   Funcitons 
--************************************************************************************
function SpawnPet(self, num ) 
     for i = 1,  self:GetVar("TotalPets")  do  
        if num == i then
             local firstWP = GAMEOBJ:GetWaypointPos( self:GetVar("WP_Alpha").."_"..i, 1)
             self:SetVar("ChildLoadNUM", i ) 
             RESMGR:LoadObject { objectTemplate =  self:GetVar( "Pet_"..i )  , x=  firstWP.x  , y=  firstWP.y , z=  firstWP.z  , owner = self } -- A1  
             self:SetVar(self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..i, "Spawned" )  
             local NChildern =  self:GetVar("NumberOfChildern") + 1 
             self:SetVar("NumberOfChildern", NChildern ) 
             break
        end 
        
     end
     if  self:GetVar("NumberOfChildern") < self:GetVar("MaxPets") + 1 then
           setState("ReSpawnChild",self)
     end 
end

function GetValidRandom(self) 

    while true do 
         NUM  = math.random(1,self:GetVar("TotalPets"))  
         FinalName = (self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..NUM)
        if self:GetVar(FinalName) == "NotSpawned" then 
            break
        end
    end 
    return NUM 
end 









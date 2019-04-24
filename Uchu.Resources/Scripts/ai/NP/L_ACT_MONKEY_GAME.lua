--------------------------------------------------------------
-- (SERVER SIDE) Script for the Lead the Monkey Minigame
--
-- :: "spawn_path" config data needs to be set for the grid path per puzzle
-- :: assumes triggers are setup for 4 distinct buttons to send fire events
--    to the board. event name is "button_activate". sender's LOT is used
-- :: assumes spawn_path is setup correctly with points going left to right
--    then top to bottom. ex: 1 2 3 4 || 5 6 7 8 || 9 10 11 12 || 13 14 15 16
--
-- Is setup as an activity to give rewards to a single user.
--------------------------------------------------------------


--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- Locals and Constants
--------------------------------------------------------------
CONSTANTS = {}
CONSTANTS["MONKEYMINI_PROX_RADIUS"] = 1.0
CONSTANTS["MONKEYMINI_LEVEL_CLEAR_DELAY"] = 5.0
CONSTANTS["MONKEYMINI_BUTTON_LOTS"] = {3704, 3705, 3706, 3707}  -- order dictates buttons number 1/2/3/4
CONSTANTS["MONKEYMINI_FLOOR_LOTS"] = {3747, 3748, 3749, 3750}  -- order dictates item floors 1/2/3/4
CONSTANTS["MONKEYMINI_NUM_BUTTONS"] = #CONSTANTS["MONKEYMINI_BUTTON_LOTS"]
CONSTANTS["MONKEYMINI_FLOOR_OFFSET"] = -0.2
CONSTANTS["MONKEYMINI_MAX_OBSTACLES"] = 4
CONSTANTS["MONKEYMINI_GOAL_LOT"] = 3708
CONSTANTS["MONKEYMINI_ITEM_LOT"] = 3709
CONSTANTS["MONKEYMINI_MONKEY_LOT"] = 3710
CONSTANTS["MONKEYMINI_OBSTACLE_LOT"] = 3737
CONSTANTS["MONKEYMINI_TIMEOUT"] = 30.0  -- value for timing out the user


LEVEL_DATA = {}
-------------------
--B1| 01 02 03 04 |
--B2| 05 06 07 08 |
--B3| 09 10 11 12 |
--B4| 13 14 15 16 |
-------------------

LEVEL_DATA[1] = { start = 2,  goal = 15,  b1 = 5,  b2 = 7,  b3 = 14, b4 = 16, o1 = 6, o2 = 12, o3 = 0, o4 = 0 }
LEVEL_DATA[2] = { start = 12,  goal = 4,   b1 = 4,  b2 = 5, b3 = 13,  b4 = 15, o1 = 8, o2 = 11, o3 = 6, o4 = 14 }
LEVEL_DATA[3] = { start = 9,  goal = 7,   b1 = 4,  b2 = 13, b3 = 14,  b4 = 16, o1 = 10, o2 = 3, o3 = 11, o4 = 0 }
LEVEL_DATA[4] = { start = 9,  goal = 7,   b1 = 4,  b2 = 13, b3 = 14,  b4 = 12, o1 = 2, o2 = 6, o3 = 10, o4 = 0 }
LEVEL_DATA[5] = { start = 1,  goal = 11,   b1 = 1,  b2 = 4, b3 = 13,  b4 = 16, o1 = 7, o2 = 10, o3 = 6, o4 = 0 }
CONSTANTS["MONKEYMINI_LEVEL_DATA"] = LEVEL_DATA

-- debug settings
local show_debug = false
local load_test_level = false
local test_level = 5 

--------------------------------------------------------------
-- Helper Functions
--------------------------------------------------------------

--------------------------------------------------------------
-- Display debug output
--------------------------------------------------------------
function DebugOutput(strVal)
    if (show_debug and show_debug == true) then
        print(strVal)
    end
end


--------------------------------------------------------------
-- Checks the LOT of an object against valid buttons
-- and returns the button number 1,2,3,4 in the list
-- returns nil otherwise
--------------------------------------------------------------
function GetButtonNumber(self, objID)

    if (CONSTANTS["MONKEYMINI_BUTTON_LOTS"] == nil) then
        return nil
    end
    
    -- look for a valid button
	for button = 1, CONSTANTS["MONKEYMINI_NUM_BUTTONS"] do
		if (objID:GetLOT().objtemplate == CONSTANTS["MONKEYMINI_BUTTON_LOTS"][button]) then
			return button
		end
	end

	return nil

end


--------------------------------------------------------------
-- Returns true if template is a floor object
--------------------------------------------------------------
function IsFloorObject(self, templateID)
    -- look for a valid floor
	for floor = 1, CONSTANTS["MONKEYMINI_NUM_BUTTONS"] do
		if (templateID == CONSTANTS["MONKEYMINI_FLOOR_LOTS"][floor]) then
			return true
		end
	end
	return false
end	


--------------------------------------------------------------
-- Gets a path from config data and tests it
--------------------------------------------------------------
function HasValidPath(self)

	local bPathFound = false
	local pathName = self:GetVar("spawn_path")
	if (pathName) then
		local pathMsg = LEVEL:GetPathWaypoints(pathName)
		if (tostring(type(pathMsg)) == "table") then
		    -- @TODO: get max waypoints and store for validity?
			bPathFound = true
		end	
	end
	
	return bPathFound

end


--------------------------------------------------------------
-- Get the state of a button
--------------------------------------------------------------
function GetButtonState(self, buttonNum)

    local button_states = self:GetVar("ButtonStates")
    return button_states[buttonNum]

end


--------------------------------------------------------------
-- Set the state of a button
--------------------------------------------------------------
function SetButtonState(self, buttonNum, state)

    local button_states = self:GetVar("ButtonStates")
    button_states[buttonNum] = state
    self:SetVar("ButtonStates", button_states)

end


--------------------------------------------------------------
-- Adds an object to the board at the specified waypoint pos
--------------------------------------------------------------
function AddObject(self, templateID, wpPos)
    -- get waypoint position
    local pathName = self:GetVar("spawn_path")
    local objectPos = GAMEOBJ:GetWaypointPos( pathName, tonumber(wpPos) )

    -- special processing for floors
    if (IsFloorObject(self, templateID) == true) then
    
        objectPos.y = objectPos.y + CONSTANTS["MONKEYMINI_FLOOR_OFFSET"]
        
    end
    
    -- load the object in the world
    RESMGR:LoadObject { objectTemplate = templateID,
                        x = objectPos.x,
                        y = objectPos.y,
                        z = objectPos.z,
                        owner = self }    
end


--------------------------------------------------------------
-- Randomizes the level buttons and returns levelData
--------------------------------------------------------------
function RandomizeLevelButtons(self, levelData)

    -- randomize the button locations before storing the data

    -- fill the temp table with values from the level
    local tempTable = {}
	for button = 1, CONSTANTS["MONKEYMINI_NUM_BUTTONS"] do
	    local buttonStr = "b" .. button
	    tempTable[button] = tonumber(levelData[buttonStr])
	end
	
	-- shuffle the values into a new table
	local count = 1
	local newTable = {}
	while (#tempTable) and (#tempTable > 0) do
        local ranVal = math.random(1, #tempTable)
        newTable[count] = tempTable[ranVal]
        table.remove(tempTable, ranVal)
        count = count + 1
    end

    -- store the new table back in level data
	for button = 1, CONSTANTS["MONKEYMINI_NUM_BUTTONS"] do
	    local buttonStr = "b" .. button
	    levelData[buttonStr] = tonumber(newTable[button])
	end

    return levelData    

end


--------------------------------------------------------------
-- Clears the board of objects
--------------------------------------------------------------
function ClearLevel(self)

    RemoveObject(self, "GoalObject")
    RemoveObject(self, "MonkeyObject")

    -- remove obstacles
	for obstacle = 1, CONSTANTS["MONKEYMINI_MAX_OBSTACLES"] do
	    RemoveObject(self, "Obstacle" .. obstacle)
	end

    -- remove floors
	for floor = 1, CONSTANTS["MONKEYMINI_NUM_BUTTONS"] do
	    RemoveObject(self, "Floors" .. floor)
	end
	
    -- remove items    
	for button = 1, CONSTANTS["MONKEYMINI_NUM_BUTTONS"] do
	    RemoveObject(self, "Item" .. button)
	    SetButtonState(self, button, false)
	end

end


--------------------------------------------------------------
-- Returns the number of the button that spawned an item 
-- based on the waypoint position of that item
--------------------------------------------------------------
function FindButtonNumByPos(self, wpPos)

    -- get level data
    local levelData = self:GetVar("LevelData")
    
    -- find the item based on the waypoint pos
	for button = 1, CONSTANTS["MONKEYMINI_NUM_BUTTONS"] do
	    local bstring = "b" .. button
	    if (tonumber(levelData[bstring]) == tonumber(wpPos)) then
	        return button
	    end
	end

    -- not found
    return nil
    
end


--------------------------------------------------------------
-- Removes a stored object by name
--------------------------------------------------------------
function RemoveObject(self, objName)

    local oldChild = getObjectByName(self, objName)
    if (oldChild ~= nil and oldChild:Exists()) then
        GAMEOBJ:DeleteObject(oldChild)
    end

end


--------------------------------------------------------------
-- Selects a random level and loads it on the board
--------------------------------------------------------------
function LoadLevel(self, bReset)

    -- clear old objects off the board
    ClearLevel(self)
    InitVars(self)
    
    -- get the last level
    local lastLevel = tonumber(self:GetVar("LastLevel"))
    local thisLevel = lastLevel

    -- select another level
    while ( (load_test_level == false) and (bReset == false) and (lastLevel == thisLevel) ) do
        thisLevel = math.random(1, #CONSTANTS["MONKEYMINI_LEVEL_DATA"])
    end
    
    if (load_test_level == true) then
        thisLevel = test_level
    end
    
    DebugOutput("Setting monkey minigame to level " .. thisLevel)
    local levelData = CONSTANTS["MONKEYMINI_LEVEL_DATA"][thisLevel]

    -- if we are making a new level, randomize button locations    
    if (bReset == false) then
        levelData = RandomizeLevelButtons(self, levelData)
    end
    
    -- store the level data
    self:SetVar("LevelData", levelData)
    self:SetVar("LastLevel", thisLevel)
    
    -- spawn objects
    AddObject(self, CONSTANTS["MONKEYMINI_GOAL_LOT"], levelData.goal)
    AddObject(self, CONSTANTS["MONKEYMINI_MONKEY_LOT"], levelData.start)
    
    -- spawn obstacles
	for obstacle = 1, CONSTANTS["MONKEYMINI_MAX_OBSTACLES"] do
	    local obsStr = "o" .. obstacle
	    if (levelData[obsStr] and tonumber(levelData[obsStr]) > 0) then
	        AddObject(self, CONSTANTS["MONKEYMINI_OBSTACLE_LOT"], levelData[obsStr])
	    end
	end    
	
	-- spawn floors
	for button = 1, CONSTANTS["MONKEYMINI_NUM_BUTTONS"] do
	    local bstring = "b" .. button
	    AddObject(self, CONSTANTS["MONKEYMINI_FLOOR_LOTS"][button], levelData[bstring])
	end	
    
end


--------------------------------------------------------------
-- Records the waypoint for the button pressed. Used to 
-- make the monkey move once the waypoint list is full
--------------------------------------------------------------
function AddOrderWaypoint(self, itemPos)

    local wpOrder = self:GetVar("WaypointOrder")
    local wpSize = self:GetVar("WaypointSize")

    wpSize = wpSize + 1
    
    -- store waypoints
    wpOrder[wpSize] = itemPos
    self:SetVar("WaypointOrder", wpOrder)
    self:SetVar("WaypointSize", wpSize)

    -- if our waypoints are full, start the action
    if (wpSize == CONSTANTS["MONKEYMINI_NUM_BUTTONS"]) then
        StartAction(self)
    end
    
end


--------------------------------------------------------------
-- Gets a waypoint out of the list for the monkey
--------------------------------------------------------------
function GetOrderWaypoint(self, wpNum)

    -- get the waypoints
    local wpOrder = self:GetVar("WaypointOrder")
    local wpSize = self:GetVar("WaypointSize")

    if (tonumber(wpNum) <= 0 or tonumber(wpNum) > tonumber(wpSize)) then
        print("ERROR: invalid waypoint number for ordered list on monkey")
        return nil
    end

    -- get the exact waypoint pos
    return wpOrder[wpNum]        

end


--------------------------------------------------------------
-- Tells the monkey to goto a waypoint
--------------------------------------------------------------
function MoveMonkeyToOrderWaypoint(self, curWaypoint)

    DebugOutput("MOVING MONKEY TO order point " .. curWaypoint)
    
    -- are we done with waypoints?
    if (tonumber(curWaypoint) > CONSTANTS["MONKEYMINI_NUM_BUTTONS"]) then

        -- then we fail
        FailLevel(self)
        
    else
        
        self:SetVar("CurWP", curWaypoint)
        
        -- get the position from this order waypoint
        local wpPos = GetOrderWaypoint(self, curWaypoint)

        -- get waypoint position
        local pathName = self:GetVar("spawn_path")
        local objectPos = GAMEOBJ:GetWaypointPos( pathName, tonumber(wpPos) )

        -- get the monkey
        local monkey = getObjectByName(self, "MonkeyObject")
        
        -- move the monkey
        monkey:GoTo{target = objectPos}

    end
    
end


--------------------------------------------------------------
-- Start moving the monkey through waypoints
--------------------------------------------------------------
function StartAction(self)

    DebugOutput("STARTING THE MOVE")
    
    -- cancel the user timeout
    GAMEOBJ:GetTimer():CancelTimer("UserTimeout", self)
    
    self:SetVar("InAction", true)
    
    -- start on the first waypoint
    MoveMonkeyToOrderWaypoint(self, 1)
        
end


--------------------------------------------------------------
-- Monkey touched the goal
--------------------------------------------------------------
function CompleteLevel(self)

    DebugOutput("LEVEL COMPLETE")
    
    self:SetVar("InAction", false)
    
    -- stop the monkey and animate
    local monkey = getObjectByName(self, "MonkeyObject")
    monkey:StopPathing()
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "PlayWinAnimation", monkey )    
    
    -- reward user
    self:DistributeActivityRewards()
    
    -- call activity complete
    self:CompleteActivity()
    
    -- @TODO: add timer to reset level
    GAMEOBJ:GetTimer():CancelAllTimers( self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["MONKEYMINI_LEVEL_CLEAR_DELAY"], "StartNewLevel",self )    

end


--------------------------------------------------------------
-- Monkey failed to touch the goal
--------------------------------------------------------------
function FailLevel(self)

    DebugOutput("LEVEL FAILED")
    
    self:SetVar("InAction", false)

    -- stop the monkey and animate
    local monkey = getObjectByName(self, "MonkeyObject")
    monkey:StopPathing()
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "PlayLoseAnimation", monkey )    
    
    -- @TODO: add timer to reset level
    GAMEOBJ:GetTimer():CancelAllTimers( self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["MONKEYMINI_LEVEL_CLEAR_DELAY"], "RestartLevel",self )    

end

--------------------------------------------------------------
-- Initialize vars for each use
--------------------------------------------------------------
function InitVars(self)

    -- clear all users
    self:RemoveActivityUser()
    
	-- stores the state of buttons, true means it has been pressed
	local button_states = {}
	local wp_order = {}
	for button = 1, CONSTANTS["MONKEYMINI_NUM_BUTTONS"] do
		button_states[button] = false
		wp_order[button] = 0
	end
	self:SetVar("ButtonStates", button_states)
	
	-- stores the order of waypoints to travel for the monkey
	self:SetVar("WaypointOrder", wp_order)
	self:SetVar("WaypointSize", 0)
	
	-- num obstacles loaded
	self:SetVar("NumObstaclesLoaded", 0)

    -- num floors loaded
	self:SetVar("NumFloorsLoaded", 0)

	-- stores if the activity is in use by a player
	self:SetVar("InUse", false)
	
	-- stores if the monkey is in action
	self:SetVar("InAction", false)
	
	-- time since user started
	self:SetVar("StartTime", 0)

end


--------------------------------------------------------------
-- Game Message Handlers
--------------------------------------------------------------

--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self) 

	-- init vars
	InitVars(self)

	-- stores the last level loaded, so we init it here to 0 (only time)
	self:SetVar("LastLevel", 0)	

	-- @TODO: need a Ready var?
	
	
	-- test for valid path
	if (HasValidPath(self) == false) then
		print("ERROR: spawn_path config data not contain a valid path for monkey minigame.")
	else

	    -- startup the game by loading a random level
	    LoadLevel(self, false)
	    
    end
	
end


--------------------------------------------------------------
-- checks to see if the param user is the current correct user for the
-- activity. If it is not, will try to add the user as the current user
-- for the activity. Returns true if the user was added or is correct
--------------------------------------------------------------
function IsCorrectUser(self, user)
 
    -- if we have no users, add this user as our user
    if (tonumber(self:GetNumActivityUsers().numUsers) == 0) then
       self:AddActivityUser{ userID = user }

        -- start a timer to time-out for the user?
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["MONKEYMINI_TIMEOUT"], "UserTimeout",self )            
        
        -- Store the start time for the user
        local startTime = GAMEOBJ:GetSystemTime()
        self:SetVar("StartTime", startTime)
        
       return true
    
    -- if this is not the right user, quit out
    elseif (self:ActivityUserExists{ userID = user }.bExists == false) then
        return false
    end 
    
    return true

end


--------------------------------------------------------------
-- Called when an event is fired onto the object
--------------------------------------------------------------
function onFireEvent(self, msg)

    if (msg.args == "button_activate") then

        -- validate the user pressing the button    
        local buttonUser = getObjectByName(msg.senderID, "myUser")
        if (not buttonUser) then
            return
        end

        -- check to see if this is the correct user, or a new user
        -- will add user if new
        if (IsCorrectUser(self, buttonUser) == false) then
            return
        end
    
        -- find out what button was pressed, and if that button has not been pressed yet
        local buttonNumber = GetButtonNumber(self, msg.senderID)
        if ( (buttonNumber) and (GetButtonState(self, buttonNumber) == false) ) then

            -- set button as pressed (to disable further pressing)
            SetButtonState(self, buttonNumber, true)
            DebugOutput("pressed button " .. buttonNumber)
            
            -- store this as the last button pressed
            storeObjectByName(self, "LastButton", msg.senderID)

            -- get the level data and add an item to the board
            local levelData = self:GetVar("LevelData")
            local itemPos = levelData["b" .. buttonNumber]
            AddObject(self, CONSTANTS["MONKEYMINI_ITEM_LOT"], tonumber(itemPos))
            
            -- press the button
            msg.senderID:PlayFXEffect{effectType = "press"}

            -- add this position to the order of waypoints to travel
            -- also starts the action process if the waypoints are full
            AddOrderWaypoint(self, itemPos)
                
        end
       
    elseif (msg.args == "monkey_prox") then 
 
        -- did the monkey run into the goal?
        if ( (self:GetVar("InAction") == true) and 
             (msg.senderID:GetLOT().objtemplate == CONSTANTS["MONKEYMINI_GOAL_LOT"]) ) then
        
            CompleteLevel(self)
        
        -- did the monkey run into an obstacle
        elseif ( (self:GetVar("InAction") == true) and 
             (msg.senderID:GetLOT().objtemplate == CONSTANTS["MONKEYMINI_OBSTACLE_LOT"]) ) then
        
            FailLevel(self)
            
        end
 
    elseif (msg.args == "monkey_arrived") then 
 
        if (self:GetVar("InAction") == true) then
            
            -- get the waypoint and remove the item
            local curWp = self:GetVar("CurWP")
            local wpPos = GetOrderWaypoint(self, curWp)
            local itemNum = FindButtonNumByPos(self, wpPos)
            RemoveObject(self, "Item" .. itemNum)
            
            -- goto next waypoint
            curWp = curWp + 1
            MoveMonkeyToOrderWaypoint(self, curWp)
            
        end
 
    end
    
end


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)

    -- is this a goal object
	if msg.templateID == CONSTANTS["MONKEYMINI_GOAL_LOT"] then 

        -- remove old object, add new
        RemoveObject(self, "GoalObject")
    	storeObjectByName(self, "GoalObject", msg.childID)
    	
    -- is this the monkey object    	
    elseif msg.templateID == CONSTANTS["MONKEYMINI_MONKEY_LOT"] then 
    
        -- remove old object, add new
        RemoveObject(self, "MonkeyObject")
    	storeObjectByName(self, "MonkeyObject", msg.childID)
    	msg.childID:SetProximityRadius{ radius = CONSTANTS["MONKEYMINI_PROX_RADIUS"] }

		-- store who the parent is
		storeParent(self, msg.childID)

    -- is this an item object created from a button
    elseif msg.templateID == CONSTANTS["MONKEYMINI_ITEM_LOT"] then 

        -- get the last button
        local lastButton = getObjectByName(self, "LastButton")
        if (lastButton ~= nil) then
        
            -- store this item based on the button that created it
            local buttonNumber = GetButtonNumber(self, lastButton)
            if (buttonNumber) then
            
                local itemStr = "Item" .. buttonNumber
                RemoveObject(self, itemStr)
                storeObjectByName(self, itemStr, msg.childID)

            end  
        
        end

    -- is this an obstacle
    elseif msg.templateID == CONSTANTS["MONKEYMINI_OBSTACLE_LOT"] then 

        -- get the last obstacle
        local lastObstacle = tonumber(self:GetVar("NumObstaclesLoaded"))
        if (lastObstacle ~= nil) then
        
            -- increase obstacles loaded
            lastObstacle = lastObstacle + 1
            self:SetVar("NumObstaclesLoaded", lastObstacle)
            
            -- store this one
            local itemStr = "Obstacle" .. lastObstacle
            RemoveObject(self, itemStr)
            storeObjectByName(self, itemStr, msg.childID)
            msg.childID:SetProximityRadius{ radius = 4 }
            
            -- store who the parent is
            storeParent(self, msg.childID)
            
        end

    -- is this a floor    
    elseif IsFloorObject(self, msg.templateID) then
        
        -- get the last floor
        local lastFloor = tonumber(self:GetVar("NumFloorsLoaded"))
        if (lastFloor ~= nil) then
        
            -- increase floors loaded
            lastFloor = lastFloor + 1
            self:SetVar("NumFloorsLoaded", lastFloor)
            
            -- store this one
            local itemStr = "Floor" .. lastFloor
            RemoveObject(self, itemStr)
            storeObjectByName(self, itemStr, msg.childID)
        
        end        
    
    end

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone(self, msg)
	
    if (msg.name == "StartNewLevel") then
    
        LoadLevel(self, false)
    
    elseif (msg.name == "RestartLevel") then
    
        LoadLevel(self, true)

    elseif (msg.name == "UserTimeout") then
    
        FailLevel(self)
    
    end
	
end


--------------------------------------------------------------
-- Called when the activity is trying to calculate final rating
--------------------------------------------------------------
function onDoCalculateActivityRating(self, msg)

    -- calculate total time spent for user as the rating
    local endTime = GAMEOBJ:GetSystemTime()
    local startTime = self:GetVar("StartTime")
    local totalTime = tonumber(endTime) - tonumber(startTime)
    msg.outActivityRating = totalTime
    return msg

end


--------------------------------------------------------------
-- Called when the activity is trying completed
--------------------------------------------------------------
function onDoCompleteActivityEvents(self, msg)

    -- @TODO: add any completion events here
    
    msg.userID:DisplayTooltip{ bShow = true, strText = "You completed the monkey game!", iTime = 5000 }
   
end    
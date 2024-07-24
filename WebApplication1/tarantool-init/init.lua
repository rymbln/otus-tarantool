#!/usr/bin/env tarantool

-- Add Taranrocks pathes. https://github.com/rtsisyk/taranrocks/blob/master/README.md
local home = os.getenv("HOME")
local log = require('log')
pg = require('pg')
driver = require('pg.driver')

local conn_str = 'postgresql://'..os.getenv('PG_USER')..':'..os.getenv('PG_PASSWORD')
        ..'@'..os.getenv('PG_HOST')..':'..os.getenv('PG_PORT')
        ..'/'..os.getenv('PG_DATABASE')
log.info(conn_str)

box.cfg
{
    pid_file = nil,
    background = false,
    log_level = 5
}

local function init()
    box.schema.user.create('tarantool', {password = 'tarantool', if_not_exists = true})
    box.schema.user.grant('tarantool', 'read,write,execute', 'universe', nil, { if_not_exists = true })

    box.schema.space.create('users', { if_not_exists = true, temporary = true })

    -- Primary key definition (assuming 'id' as primary key)
    box.space.users:format({
        { name = 'id', type = 'number' },
        { name = 'first_name', type = 'string' },
        { name = 'last_name', type = 'string' },
        { name = 'email', type = 'string' },
        { name = 'gender', type = 'string' },
        { name = 'birthdate', type = 'string' }, -- Unix timestamp or string format
        { name = 'is_active', type = 'boolean' },
        { name = 'ip_address', type = 'string' },
        { name = 'score', type = 'unsigned' },
        { name = 'created', type = 'string' }, -- Unix timestamp or string format
        { name = 'unique_id', type = 'string' }
    })

    -- Create an auto-increment primary key
    box.space.users:create_index('primary', {
        type = 'tree',
        parts = { 'id' },
        unique = true,
        if_not_exists = true
    })

    -- Create an index on the 'email' column
    box.space.users:create_index('email_index', {
        type = 'hash',
        parts = { 'email' },
        unique = true,
        if_not_exists = true
    })
    log.info("Init completed")
end

function reload_single()
    log.info("Prepare truncate")
    box.space.users:truncate()
    log.info("Truncated")
    -- Using a fiber for batch insertion
    local fiber = require('fiber')
    local status, pg_conn = driver.connect(conn_str)
    if status < 0 then
        log.info(pg_conn)
    else
        log.info("Connected")
        local status, result = pg_conn:execute("SELECT id, first_name, last_name, email, gender, birthdate, is_active, ip_address, score, created, unique_id FROM public.users order by id")

        if status < 0 then
            log.info(result)
        else
            log.info("Start processing %s", #result[1])

            -- box.begin()

            local users = result[1]
            local total_count = 0

            for _, user in ipairs(users) do
                local id = user['id']
                local first_name = user['first_name']
                local last_name = user['last_name']
                local email = user['email']
                local gender = user['gender']
                local birthdate = user['birthdate']
                local is_active = user['is_active']
                local ip_address = user['ip_address']
                local score = user['score']
                local created = user['created']
                local unique_id = user['unique_id']

                -- Create a Lua table for each user
                local user_entry = {
                    id,
                    first_name,
                    last_name,
                    email,
                    gender,
                    birthdate,
                    is_active,
                    ip_address,
                    score,
                    created,
                    unique_id
                }

                local ok, err = pcall(function()
                    box.space.users:insert(user_entry)
                    fiber.yield()
                end)
                if not ok then
                    log.info("Error inserting batch: %s. %s", id, err)
                end

                total_count = total_count + 1
            end

            -- box.commit()

            pg_conn:close()
            log.info("Loaded: %s", total_count)
            return count
        end
    end
end

-- Function to insert a new user into the 'users'
function create_user(id, first_name, last_name, email, gender, birthdate, is_active, ip_address, score, created, unique_id)
    local user = {
        id = id,
        first_name = first_name,
        last_name = last_name,
        email = email,
        gender = gender,
        birthdate = birthdate,
        is_active = is_active,
        ip_address = ip_address,
        score = score,
        created = created,
        unique_id = unique_id
    }
    return box.space.users:insert(user)
end

-- Function to retrieve a user by their ID
function read_user(id)
    return box.space.users:select{id}
end

-- Function to update a user's information
function update_user(id, updates)
    local user = box.space.users:update(id, updates)
    if user ~= nil then
        return true
    else
        return false
    end
end

-- Function to delete a user by their ID
function delete_user(id)
    return box.space.users:delete{id}
end

-- Function to retrieve a user by their email address
function read_user_by_email(email)
    local index_key = { email }
    local user = box.space.users.index.email_index:select(index_key)
    if #user > 0 then
        return user[1] -- Assuming email is unique, return the first matching user
    else
        return nil
    end
end

box.once('init', init)
log.info("Script ended")
print("Script ended print")
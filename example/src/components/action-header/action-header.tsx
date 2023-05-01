import React from 'react'
import {Box, HStack, Center, Text, Input} from '@chakra-ui/react'

type Props = {}

const ActionHeader = (props: Props) => {
  return (
    <HStack spacing={'50px'} bg={'cornflowerblue'} w={'100%'} h={'100px'}>
        <Center color={'white'} padding={'10px'}>
            <Text padding={'10px'}>Action</Text>
            <Text padding={'10px'}>Header</Text>
            <Input bgColor={'whiteAlpha.900'} color={'black'} defaultValue={"dhsjkdhslkj"}></Input>
        </Center>
    </HStack>
  )
}

export default ActionHeader
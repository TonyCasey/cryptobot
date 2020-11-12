import { BotResponseDto, IBotResponseDto } from './dto';
import { Exchange, Coin } from '.';
import { Position } from './position.model';

export class Bot extends BotResponseDto {

    exchange: Exchange;
    coin?: Coin;
    baseCoin?: Coin;
    positions: Position[] = [];


}
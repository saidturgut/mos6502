; setup pointer for (zp),Y
        LDA #<table
        STA $FF
        LDA #>table
        STA $00

        LDY #$01

        ; load via (zp),Y
        LDA ($FF),Y        ; A = table[1]
        PHA                ; save it

        ; absolute,X test
        LDX #$01
        LDA table,X      ; same value as above
        CMP ($FF),Y
        BNE fail

        ; stack + flags
        PLA                ; restore A
        CMP #$42
        BEQ ok

fail:
        BRK                ; should not hit

ok:
        ; subroutine + indirect jump
        JSR subbb
        JMP (jmpvec)

subbb:
        RTS

table:
        .byte $10, $42, $99

jmpvec:
        .word done

done:
     DB $ff
     